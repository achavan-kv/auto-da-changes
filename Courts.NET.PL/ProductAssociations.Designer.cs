namespace STL.PL
{
    partial class Product_Associations
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
            this.btnClear = new System.Windows.Forms.Button();
            this.lbAssociatedItems = new System.Windows.Forms.Label();
            this.btnReload = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lbAssociate = new System.Windows.Forms.Label();
            this.txtAssociatedItem = new System.Windows.Forms.TextBox();
            this.dgvAssociatedItems = new System.Windows.Forms.DataGridView();
            this.drpSubClass = new System.Windows.Forms.ComboBox();
            this.lbSubClass = new System.Windows.Forms.Label();
            this.drpClass = new System.Windows.Forms.ComboBox();
            this.lbClass = new System.Windows.Forms.Label();
            this.drpCategory = new System.Windows.Forms.ComboBox();
            this.lbCategory = new System.Windows.Forms.Label();
            this.lbProductGroup = new System.Windows.Forms.Label();
            this.drpProductGroup = new System.Windows.Forms.ComboBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAssociatedItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(693, 84);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 16;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lbAssociatedItems
            // 
            this.lbAssociatedItems.AutoSize = true;
            this.lbAssociatedItems.Location = new System.Drawing.Point(17, 106);
            this.lbAssociatedItems.Name = "lbAssociatedItems";
            this.lbAssociatedItems.Size = new System.Drawing.Size(87, 13);
            this.lbAssociatedItems.TabIndex = 15;
            this.lbAssociatedItems.Text = "Associated Items";
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(693, 27);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(75, 23);
            this.btnReload.TabIndex = 14;
            this.btnReload.Text = "ReLoad";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(693, 54);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 13;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.Gray;
            this.btnAdd.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnAdd.Image = global::STL.PL.Properties.Resources.plus;
            this.btnAdd.Location = new System.Drawing.Point(649, 32);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(20, 20);
            this.btnAdd.TabIndex = 11;
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
            this.btnDelete.Location = new System.Drawing.Point(649, 55);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(20, 20);
            this.btnDelete.TabIndex = 12;
            this.btnDelete.TabStop = false;
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lbAssociate
            // 
            this.lbAssociate.AutoSize = true;
            this.lbAssociate.Location = new System.Drawing.Point(531, 38);
            this.lbAssociate.Name = "lbAssociate";
            this.lbAssociate.Size = new System.Drawing.Size(76, 13);
            this.lbAssociate.TabIndex = 10;
            this.lbAssociate.Text = "Associate Item";
            // 
            // txtAssociatedItem
            // 
            this.txtAssociatedItem.Location = new System.Drawing.Point(531, 56);
            this.txtAssociatedItem.Name = "txtAssociatedItem";
            this.txtAssociatedItem.Size = new System.Drawing.Size(108, 20);
            this.txtAssociatedItem.TabIndex = 9;
            this.txtAssociatedItem.Leave += new System.EventHandler(this.txtAssociatedItem_Leave);
            // 
            // dgvAssociatedItems
            // 
            this.dgvAssociatedItems.AllowUserToAddRows = false;
            this.dgvAssociatedItems.AllowUserToResizeColumns = false;
            this.dgvAssociatedItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAssociatedItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAssociatedItems.Location = new System.Drawing.Point(12, 126);
            this.dgvAssociatedItems.MultiSelect = false;
            this.dgvAssociatedItems.Name = "dgvAssociatedItems";
            this.dgvAssociatedItems.ReadOnly = true;
            this.dgvAssociatedItems.Size = new System.Drawing.Size(768, 322);
            this.dgvAssociatedItems.TabIndex = 8;
            this.dgvAssociatedItems.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgvAssociatedItems_MouseUp);
            // 
            // drpSubClass
            // 
            this.drpSubClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpSubClass.FormattingEnabled = true;
            this.drpSubClass.Location = new System.Drawing.Point(355, 57);
            this.drpSubClass.Name = "drpSubClass";
            this.drpSubClass.Size = new System.Drawing.Size(161, 21);
            this.drpSubClass.TabIndex = 7;
            this.drpSubClass.SelectedIndexChanged += new System.EventHandler(this.drpSubClass_SelectedIndexChanged);
            // 
            // lbSubClass
            // 
            this.lbSubClass.AutoSize = true;
            this.lbSubClass.Location = new System.Drawing.Point(285, 58);
            this.lbSubClass.Name = "lbSubClass";
            this.lbSubClass.Size = new System.Drawing.Size(54, 13);
            this.lbSubClass.TabIndex = 6;
            this.lbSubClass.Text = "Sub Class";
            // 
            // drpClass
            // 
            this.drpClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpClass.FormattingEnabled = true;
            this.drpClass.Location = new System.Drawing.Point(95, 55);
            this.drpClass.Name = "drpClass";
            this.drpClass.Size = new System.Drawing.Size(161, 21);
            this.drpClass.TabIndex = 5;
            this.drpClass.SelectedIndexChanged += new System.EventHandler(this.drpClass_SelectedIndexChanged);
            // 
            // lbClass
            // 
            this.lbClass.AutoSize = true;
            this.lbClass.Location = new System.Drawing.Point(56, 57);
            this.lbClass.Name = "lbClass";
            this.lbClass.Size = new System.Drawing.Size(32, 13);
            this.lbClass.TabIndex = 4;
            this.lbClass.Text = "Class";
            // 
            // drpCategory
            // 
            this.drpCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCategory.FormattingEnabled = true;
            this.drpCategory.Location = new System.Drawing.Point(354, 26);
            this.drpCategory.Name = "drpCategory";
            this.drpCategory.Size = new System.Drawing.Size(161, 21);
            this.drpCategory.TabIndex = 3;
            this.drpCategory.SelectedIndexChanged += new System.EventHandler(this.drpCategory_SelectedIndexChanged);
            // 
            // lbCategory
            // 
            this.lbCategory.AutoSize = true;
            this.lbCategory.Location = new System.Drawing.Point(286, 29);
            this.lbCategory.Name = "lbCategory";
            this.lbCategory.Size = new System.Drawing.Size(49, 13);
            this.lbCategory.TabIndex = 2;
            this.lbCategory.Text = "Category";
            // 
            // lbProductGroup
            // 
            this.lbProductGroup.AutoSize = true;
            this.lbProductGroup.Location = new System.Drawing.Point(16, 26);
            this.lbProductGroup.Name = "lbProductGroup";
            this.lbProductGroup.Size = new System.Drawing.Size(76, 13);
            this.lbProductGroup.TabIndex = 1;
            this.lbProductGroup.Text = "Product Group";
            // 
            // drpProductGroup
            // 
            this.drpProductGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpProductGroup.FormattingEnabled = true;
            this.drpProductGroup.Location = new System.Drawing.Point(95, 24);
            this.drpProductGroup.Name = "drpProductGroup";
            this.drpProductGroup.Size = new System.Drawing.Size(161, 21);
            this.drpProductGroup.TabIndex = 0;
            this.drpProductGroup.SelectedIndexChanged += new System.EventHandler(this.drpProductGroup_SelectedIndexChanged);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // Product_Associations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.lbAssociatedItems);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lbAssociate);
            this.Controls.Add(this.txtAssociatedItem);
            this.Controls.Add(this.dgvAssociatedItems);
            this.Controls.Add(this.drpSubClass);
            this.Controls.Add(this.lbSubClass);
            this.Controls.Add(this.drpClass);
            this.Controls.Add(this.lbClass);
            this.Controls.Add(this.drpCategory);
            this.Controls.Add(this.lbCategory);
            this.Controls.Add(this.lbProductGroup);
            this.Controls.Add(this.drpProductGroup);
            this.Name = "Product_Associations";
            this.Text = "Product Associations";
            ((System.ComponentModel.ISupportInitialize)(this.dgvAssociatedItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox drpProductGroup;
        private System.Windows.Forms.Label lbProductGroup;
        private System.Windows.Forms.Label lbCategory;
        private System.Windows.Forms.ComboBox drpCategory;
        private System.Windows.Forms.Label lbClass;
        private System.Windows.Forms.ComboBox drpClass;
        private System.Windows.Forms.Label lbSubClass;
        private System.Windows.Forms.ComboBox drpSubClass;
        private System.Windows.Forms.DataGridView dgvAssociatedItems;
        private System.Windows.Forms.TextBox txtAssociatedItem;
        private System.Windows.Forms.Label lbAssociate;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Label lbAssociatedItems;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button btnClear;
    }
}