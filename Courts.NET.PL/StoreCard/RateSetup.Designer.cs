namespace STL.PL
{
    partial class StoreCardRatesSetup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StoreCardRatesSetup));
            this.lbx_ratenames = new System.Windows.Forms.ListBox();
            this.dgv_ratesetup = new System.Windows.Forms.DataGridView();
            this.btn_addnew = new System.Windows.Forms.Button();
            this.btn_delete = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.txt_ratename = new System.Windows.Forms.TextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnl_main = new System.Windows.Forms.Panel();
            this.pnl_info = new System.Windows.Forms.Panel();
            this.chk_FixedRate = new System.Windows.Forms.CheckBox();
            this.checkCashBack = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnl_buttons = new System.Windows.Forms.Panel();
            this.btn_makeDefault = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ratesetup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pnl_main.SuspendLayout();
            this.pnl_info.SuspendLayout();
            this.pnl_buttons.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbx_ratenames
            // 
            this.lbx_ratenames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbx_ratenames.FormattingEnabled = true;
            this.lbx_ratenames.Location = new System.Drawing.Point(0, 0);
            this.lbx_ratenames.Name = "lbx_ratenames";
            this.lbx_ratenames.Size = new System.Drawing.Size(116, 477);
            this.lbx_ratenames.TabIndex = 0;
            this.lbx_ratenames.SelectedIndexChanged += new System.EventHandler(this.lbx_ratenames_SelectedIndexChanged);
            // 
            // dgv_ratesetup
            // 
            this.dgv_ratesetup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_ratesetup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_ratesetup.Location = new System.Drawing.Point(0, 65);
            this.dgv_ratesetup.Name = "dgv_ratesetup";
            this.dgv_ratesetup.Size = new System.Drawing.Size(672, 363);
            this.dgv_ratesetup.TabIndex = 1;
            this.dgv_ratesetup.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_ratesetup_CellValidated);
            this.dgv_ratesetup.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_ratesetup_CellValueChanged);
            this.dgv_ratesetup.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgv_ratesetup_DataError);
            // 
            // btn_addnew
            // 
            this.btn_addnew.Location = new System.Drawing.Point(14, 13);
            this.btn_addnew.Name = "btn_addnew";
            this.btn_addnew.Size = new System.Drawing.Size(59, 23);
            this.btn_addnew.TabIndex = 2;
            this.btn_addnew.Text = "Add New";
            this.btn_addnew.UseVisualStyleBackColor = true;
            this.btn_addnew.Click += new System.EventHandler(this.btn_addnew_Click);
            // 
            // btn_delete
            // 
            this.btn_delete.Location = new System.Drawing.Point(106, 13);
            this.btn_delete.Name = "btn_delete";
            this.btn_delete.Size = new System.Drawing.Size(49, 23);
            this.btn_delete.TabIndex = 3;
            this.btn_delete.Text = "Delete";
            this.btn_delete.UseVisualStyleBackColor = true;
            this.btn_delete.Click += new System.EventHandler(this.btn_delete_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave.Location = new System.Drawing.Point(723, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 36;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnRefresh.ImageIndex = 4;
            this.btnRefresh.ImageList = this.imageList1;
            this.btnRefresh.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRefresh.Location = new System.Drawing.Point(681, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(24, 24);
            this.btnRefresh.TabIndex = 50;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            // 
            // txt_ratename
            // 
            this.txt_ratename.Location = new System.Drawing.Point(120, 15);
            this.txt_ratename.Name = "txt_ratename";
            this.txt_ratename.Size = new System.Drawing.Size(177, 20);
            this.txt_ratename.TabIndex = 51;
            this.txt_ratename.TextChanged += new System.EventHandler(this.txt_ratename_TextChanged);
            this.txt_ratename.Leave += new System.EventHandler(this.txt_ratename_Leave);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lbx_ratenames);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnl_main);
            this.splitContainer1.Panel2.Controls.Add(this.pnl_buttons);
            this.splitContainer1.Size = new System.Drawing.Size(792, 477);
            this.splitContainer1.SplitterDistance = 116;
            this.splitContainer1.TabIndex = 52;
            // 
            // pnl_main
            // 
            this.pnl_main.Controls.Add(this.dgv_ratesetup);
            this.pnl_main.Controls.Add(this.pnl_info);
            this.pnl_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_main.Location = new System.Drawing.Point(0, 49);
            this.pnl_main.Name = "pnl_main";
            this.pnl_main.Size = new System.Drawing.Size(672, 428);
            this.pnl_main.TabIndex = 2;
            this.pnl_main.Visible = false;
            // 
            // pnl_info
            // 
            this.pnl_info.Controls.Add(this.chk_FixedRate);
            this.pnl_info.Controls.Add(this.checkCashBack);
            this.pnl_info.Controls.Add(this.label1);
            this.pnl_info.Controls.Add(this.txt_ratename);
            this.pnl_info.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnl_info.Location = new System.Drawing.Point(0, 0);
            this.pnl_info.Name = "pnl_info";
            this.pnl_info.Size = new System.Drawing.Size(672, 65);
            this.pnl_info.TabIndex = 52;
            // 
            // chk_FixedRate
            // 
            this.chk_FixedRate.AutoSize = true;
            this.chk_FixedRate.Location = new System.Drawing.Point(316, 17);
            this.chk_FixedRate.Name = "chk_FixedRate";
            this.chk_FixedRate.Size = new System.Drawing.Size(115, 17);
            this.chk_FixedRate.TabIndex = 57;
            this.chk_FixedRate.Text = "Interest Rate Fixed";
            this.chk_FixedRate.UseVisualStyleBackColor = true;
            this.chk_FixedRate.CheckedChanged += new System.EventHandler(this.chk_FixedRate_CheckedChanged);
            // 
            // checkCashBack
            // 
            this.checkCashBack.AutoSize = true;
            this.checkCashBack.Location = new System.Drawing.Point(482, 18);
            this.checkCashBack.Name = "checkCashBack";
            this.checkCashBack.Size = new System.Drawing.Size(106, 17);
            this.checkCashBack.TabIndex = 56;
            this.checkCashBack.Text = "Allow Cash Back";
            this.checkCashBack.UseVisualStyleBackColor = true;
            this.checkCashBack.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 52;
            this.label1.Text = "StoreCard Rate Name";
            // 
            // pnl_buttons
            // 
            this.pnl_buttons.BackColor = System.Drawing.Color.Gainsboro;
            this.pnl_buttons.Controls.Add(this.btn_makeDefault);
            this.pnl_buttons.Controls.Add(this.btn_delete);
            this.pnl_buttons.Controls.Add(this.btn_addnew);
            this.pnl_buttons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnl_buttons.Location = new System.Drawing.Point(0, 0);
            this.pnl_buttons.Name = "pnl_buttons";
            this.pnl_buttons.Size = new System.Drawing.Size(672, 49);
            this.pnl_buttons.TabIndex = 3;
            // 
            // btn_makeDefault
            // 
            this.btn_makeDefault.Location = new System.Drawing.Point(181, 14);
            this.btn_makeDefault.Name = "btn_makeDefault";
            this.btn_makeDefault.Size = new System.Drawing.Size(116, 22);
            this.btn_makeDefault.TabIndex = 4;
            this.btn_makeDefault.Text = "Make Default Rate";
            this.btn_makeDefault.UseVisualStyleBackColor = true;
            this.btn_makeDefault.Click += new System.EventHandler(this.btn_makeDefault_Click);
            // 
            // StoreCardRatesSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.splitContainer1);
            this.Name = "StoreCardRatesSetup";
            this.Text = "StoreCard - RatesSetup";
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ratesetup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.pnl_main.ResumeLayout(false);
            this.pnl_info.ResumeLayout(false);
            this.pnl_info.PerformLayout();
            this.pnl_buttons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbx_ratenames;
        private System.Windows.Forms.DataGridView dgv_ratesetup;
        private System.Windows.Forms.Button btn_addnew;
        private System.Windows.Forms.Button btn_delete;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TextBox txt_ratename;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel pnl_main;
        private System.Windows.Forms.Panel pnl_info;
        private System.Windows.Forms.Panel pnl_buttons;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkCashBack;
        private System.Windows.Forms.Button btn_makeDefault;
        private System.Windows.Forms.CheckBox chk_FixedRate;
    }
}