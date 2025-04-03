namespace STL.PL.Warehouse
{
    partial class OnlineProductSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OnlineProductSearch));
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnOnlineProductsExcel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblLocation = new System.Windows.Forms.Label();
            this.drpLocation = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.drpOnline = new System.Windows.Forms.ComboBox();
            this.cbDateRemoved = new System.Windows.Forms.CheckBox();
            this.cbDateAdded = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.dtpDateRemoved = new System.Windows.Forms.DateTimePicker();
            this.dtpDateAdded = new System.Windows.Forms.DateTimePicker();
            this.label9 = new System.Windows.Forms.Label();
            this.drpItemCategory = new System.Windows.Forms.ComboBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.txtProdDesc = new System.Windows.Forms.TextBox();
            this.gpUpdate = new System.Windows.Forms.GroupBox();
            this.cbDCmarkAll = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.cbMarkAll = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dgProductDetails = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.gpUpdate.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgProductDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // btnOnlineProductsExcel
            // 
            this.btnOnlineProductsExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOnlineProductsExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnOnlineProductsExcel.Image")));
            this.btnOnlineProductsExcel.Location = new System.Drawing.Point(709, 11);
            this.btnOnlineProductsExcel.Name = "btnOnlineProductsExcel";
            this.btnOnlineProductsExcel.Size = new System.Drawing.Size(32, 28);
            this.btnOnlineProductsExcel.TabIndex = 144;
            this.toolTip1.SetToolTip(this.btnOnlineProductsExcel, "Export to csv");
            this.btnOnlineProductsExcel.Click += new System.EventHandler(this.btnOnlineProductsExcel_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.gpUpdate);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(787, 163);
            this.panel1.TabIndex = 10;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox3.Controls.Add(this.lblLocation);
            this.groupBox3.Controls.Add(this.drpLocation);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.drpOnline);
            this.groupBox3.Controls.Add(this.cbDateRemoved);
            this.groupBox3.Controls.Add(this.cbDateAdded);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.dtpDateRemoved);
            this.groupBox3.Controls.Add(this.dtpDateAdded);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.drpItemCategory);
            this.groupBox3.Controls.Add(this.btnExit);
            this.groupBox3.Controls.Add(this.btnClear);
            this.groupBox3.Controls.Add(this.btnSearch);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.txtProdDesc);
            this.groupBox3.Location = new System.Drawing.Point(4, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(767, 104);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Search Criteria";
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(479, 20);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(48, 13);
            this.lblLocation.TabIndex = 27;
            this.lblLocation.Text = "Location";
            // 
            // drpLocation
            // 
            this.drpLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpLocation.FormattingEnabled = true;
            this.drpLocation.Location = new System.Drawing.Point(479, 39);
            this.drpLocation.Name = "drpLocation";
            this.drpLocation.Size = new System.Drawing.Size(121, 21);
            this.drpLocation.TabIndex = 26;
            this.drpLocation.SelectedIndexChanged += new System.EventHandler(this.drpLocation_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(388, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Available Online";
            // 
            // drpOnline
            // 
            this.drpOnline.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpOnline.FormattingEnabled = true;
            this.drpOnline.Location = new System.Drawing.Point(397, 39);
            this.drpOnline.Name = "drpOnline";
            this.drpOnline.Size = new System.Drawing.Size(58, 21);
            this.drpOnline.TabIndex = 24;
            this.drpOnline.SelectedIndexChanged += new System.EventHandler(this.drpOnline_SelectedIndexChanged);
            // 
            // cbDateRemoved
            // 
            this.cbDateRemoved.AutoSize = true;
            this.cbDateRemoved.Location = new System.Drawing.Point(486, 78);
            this.cbDateRemoved.Name = "cbDateRemoved";
            this.cbDateRemoved.Size = new System.Drawing.Size(15, 14);
            this.cbDateRemoved.TabIndex = 23;
            this.cbDateRemoved.UseVisualStyleBackColor = true;
            this.cbDateRemoved.CheckedChanged += new System.EventHandler(this.cbDateRemoved_CheckedChanged);
            // 
            // cbDateAdded
            // 
            this.cbDateAdded.AutoSize = true;
            this.cbDateAdded.Location = new System.Drawing.Point(140, 78);
            this.cbDateAdded.Name = "cbDateAdded";
            this.cbDateAdded.Size = new System.Drawing.Size(15, 14);
            this.cbDateAdded.TabIndex = 22;
            this.cbDateAdded.UseVisualStyleBackColor = true;
            this.cbDateAdded.CheckStateChanged += new System.EventHandler(this.cbDateAdded_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(340, 78);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(140, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "Online Date Removed since";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 79);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(125, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Online Date Added since";
            // 
            // dtpDateRemoved
            // 
            this.dtpDateRemoved.Location = new System.Drawing.Point(507, 75);
            this.dtpDateRemoved.Name = "dtpDateRemoved";
            this.dtpDateRemoved.Size = new System.Drawing.Size(130, 20);
            this.dtpDateRemoved.TabIndex = 19;
            // 
            // dtpDateAdded
            // 
            this.dtpDateAdded.Location = new System.Drawing.Point(159, 76);
            this.dtpDateAdded.Name = "dtpDateAdded";
            this.dtpDateAdded.Size = new System.Drawing.Size(127, 20);
            this.dtpDateAdded.TabIndex = 18;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(49, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "Category";
            // 
            // drpItemCategory
            // 
            this.drpItemCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpItemCategory.FormattingEnabled = true;
            this.drpItemCategory.Location = new System.Drawing.Point(15, 39);
            this.drpItemCategory.Name = "drpItemCategory";
            this.drpItemCategory.Size = new System.Drawing.Size(128, 21);
            this.drpItemCategory.TabIndex = 15;
            this.drpItemCategory.SelectedIndexChanged += new System.EventHandler(this.drpItemCategory_SelectedIndexChanged);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(712, 69);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(48, 23);
            this.btnExit.TabIndex = 13;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(712, 37);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(48, 23);
            this.btnClear.TabIndex = 12;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.Location = new System.Drawing.Point(614, 37);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(72, 24);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(165, 23);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(145, 16);
            this.label10.TabIndex = 2;
            this.label10.Text = "Description / Courts Code";
            // 
            // txtProdDesc
            // 
            this.txtProdDesc.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtProdDesc.Location = new System.Drawing.Point(165, 39);
            this.txtProdDesc.Name = "txtProdDesc";
            this.txtProdDesc.Size = new System.Drawing.Size(210, 20);
            this.txtProdDesc.TabIndex = 2;
            this.txtProdDesc.TextChanged += new System.EventHandler(this.txtProdDesc_TextChanged);
            // 
            // gpUpdate
            // 
            this.gpUpdate.Controls.Add(this.cbDCmarkAll);
            this.gpUpdate.Controls.Add(this.btnOnlineProductsExcel);
            this.gpUpdate.Controls.Add(this.btnSave);
            this.gpUpdate.Controls.Add(this.cbMarkAll);
            this.gpUpdate.Location = new System.Drawing.Point(2, 114);
            this.gpUpdate.Name = "gpUpdate";
            this.gpUpdate.Size = new System.Drawing.Size(773, 44);
            this.gpUpdate.TabIndex = 10;
            this.gpUpdate.TabStop = false;
            this.gpUpdate.Text = "Update Selection";
            // 
            // cbDCmarkAll
            // 
            this.cbDCmarkAll.AutoSize = true;
            this.cbDCmarkAll.Location = new System.Drawing.Point(408, 20);
            this.cbDCmarkAll.Name = "cbDCmarkAll";
            this.cbDCmarkAll.Size = new System.Drawing.Size(161, 17);
            this.cbDCmarkAll.TabIndex = 145;
            this.cbDCmarkAll.Text = "Mark/Unmark all as DC Only";
            this.cbDCmarkAll.UseVisualStyleBackColor = true;
            this.cbDCmarkAll.CheckedChanged += new System.EventHandler(this.cbDCmarkAll_CheckedChanged);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(595, 14);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cbMarkAll
            // 
            this.cbMarkAll.AutoSize = true;
            this.cbMarkAll.Location = new System.Drawing.Point(53, 20);
            this.cbMarkAll.Name = "cbMarkAll";
            this.cbMarkAll.Size = new System.Drawing.Size(198, 17);
            this.cbMarkAll.TabIndex = 0;
            this.cbMarkAll.Text = "Mark/Unmark all as Available Online";
            this.cbMarkAll.UseVisualStyleBackColor = true;
            this.cbMarkAll.CheckedChanged += new System.EventHandler(this.cbMarkAll_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.dgProductDetails);
            this.panel2.Location = new System.Drawing.Point(0, 163);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(787, 332);
            this.panel2.TabIndex = 11;
            // 
            // dgProductDetails
            // 
            this.dgProductDetails.AllowUserToAddRows = false;
            this.dgProductDetails.AllowUserToDeleteRows = false;
            this.dgProductDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgProductDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgProductDetails.Location = new System.Drawing.Point(0, 0);
            this.dgProductDetails.MultiSelect = false;
            this.dgProductDetails.Name = "dgProductDetails";
            this.dgProductDetails.Size = new System.Drawing.Size(787, 332);
            this.dgProductDetails.TabIndex = 9;
            this.dgProductDetails.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgProductDetails_CellValueChanged);
            this.dgProductDetails.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgProductDetails_CurrentCellDirtyStateChanged);
            // 
            // OnlineProductSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 495);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "OnlineProductSearch";
            this.Text = "Online Product Maintenance";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.gpUpdate.ResumeLayout(false);
            this.gpUpdate.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgProductDetails)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.ComboBox drpLocation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox drpOnline;
        private System.Windows.Forms.CheckBox cbDateRemoved;
        private System.Windows.Forms.CheckBox cbDateAdded;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DateTimePicker dtpDateRemoved;
        private System.Windows.Forms.DateTimePicker dtpDateAdded;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox drpItemCategory;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtProdDesc;
        private System.Windows.Forms.GroupBox gpUpdate;
        private System.Windows.Forms.CheckBox cbDCmarkAll;
        public System.Windows.Forms.Button btnOnlineProductsExcel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox cbMarkAll;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dgProductDetails;
    }
}