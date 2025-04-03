namespace STL.PL
{
    partial class ScoringbandMatrix
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScoringbandMatrix));
            this.gbMatrix = new System.Windows.Forms.GroupBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.dtStartDate = new System.Windows.Forms.DateTimePicker();
            this.dgMatrix = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmb_scorecardtype = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.drpCountry = new System.Windows.Forms.ComboBox();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSave = new Crownwood.Magic.Menus.MenuCommand();
            this.menuIcons = new System.Windows.Forms.ImageList(this.components);
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExport = new Crownwood.Magic.Menus.MenuCommand();
            this.menuMatrix = new Crownwood.Magic.Menus.MenuCommand();
            this.menuImport = new Crownwood.Magic.Menus.MenuCommand();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.ttTermsTypeMatrix = new System.Windows.Forms.ToolTip(this.components);
            this.gbMatrix.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgMatrix)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // gbMatrix
            // 
            this.gbMatrix.BackColor = System.Drawing.SystemColors.Control;
            this.gbMatrix.Controls.Add(this.btnApply);
            this.gbMatrix.Controls.Add(this.label3);
            this.gbMatrix.Controls.Add(this.dtStartDate);
            this.gbMatrix.Controls.Add(this.dgMatrix);
            this.gbMatrix.Controls.Add(this.label1);
            this.gbMatrix.Location = new System.Drawing.Point(6, 86);
            this.gbMatrix.Name = "gbMatrix";
            this.gbMatrix.Size = new System.Drawing.Size(781, 389);
            this.gbMatrix.TabIndex = 0;
            this.gbMatrix.TabStop = false;
            this.gbMatrix.Text = "Scoreband Matrix";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(648, 19);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(90, 52);
            this.btnApply.TabIndex = 16;
            this.btnApply.TabStop = false;
            this.btnApply.Text = "Apply Service Charge %";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(391, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 20);
            this.label3.TabIndex = 15;
            this.label3.Text = "Start Date";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtStartDate
            // 
            this.dtStartDate.CustomFormat = "ddd dd MMM yyyy";
            this.dtStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtStartDate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtStartDate.Location = new System.Drawing.Point(467, 19);
            this.dtStartDate.Name = "dtStartDate";
            this.dtStartDate.Size = new System.Drawing.Size(144, 20);
            this.dtStartDate.TabIndex = 14;
            // 
            // dgMatrix
            // 
            this.dgMatrix.AllowUserToAddRows = false;
            this.dgMatrix.AllowUserToDeleteRows = false;
            this.dgMatrix.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgMatrix.Location = new System.Drawing.Point(170, 53);
            this.dgMatrix.Name = "dgMatrix";
            this.dgMatrix.Size = new System.Drawing.Size(441, 290);
            this.dgMatrix.TabIndex = 0;
            this.dgMatrix.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgMatrix_CellLeave);
            this.dgMatrix.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgMatrix_CellValidated);
            this.dgMatrix.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgMatrix_CellValidating);
            this.dgMatrix.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgMatrix_CellEndEdit);
            this.dgMatrix.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgMatrix_DataError);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(198, 158);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(384, 72);
            this.label1.TabIndex = 8;
            this.label1.Text = "You do not have sufficient permissions to view the current scoring rules.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cmb_scorecardtype);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.drpCountry);
            this.groupBox1.Location = new System.Drawing.Point(6, -1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(781, 89);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Country";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(263, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 16);
            this.label4.TabIndex = 54;
            this.label4.Text = "ScoreCard";
            // 
            // cmb_scorecardtype
            // 
            this.cmb_scorecardtype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_scorecardtype.Location = new System.Drawing.Point(266, 38);
            this.cmb_scorecardtype.Name = "cmb_scorecardtype";
            this.cmb_scorecardtype.Size = new System.Drawing.Size(168, 21);
            this.cmb_scorecardtype.TabIndex = 53;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(52, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 16);
            this.label2.TabIndex = 52;
            this.label2.Text = "Country";
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Location = new System.Drawing.Point(724, 35);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 37;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // drpCountry
            // 
            this.drpCountry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCountry.Location = new System.Drawing.Point(55, 38);
            this.drpCountry.Name = "drpCountry";
            this.drpCountry.Size = new System.Drawing.Size(168, 21);
            this.drpCountry.TabIndex = 0;
            this.drpCountry.SelectedIndexChanged += new System.EventHandler(this.drpCountry_SelectedIndexChanged);
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuSave,
            this.menuExit});
            this.menuFile.Text = "&File";
            // 
            // menuSave
            // 
            this.menuSave.Description = "MenuItem";
            this.menuSave.Enabled = false;
            this.menuSave.ImageIndex = 1;
            this.menuSave.ImageList = this.menuIcons;
            this.menuSave.Text = "&Save";
            this.menuSave.Visible = false;
            this.menuSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // menuIcons
            // 
            this.menuIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("menuIcons.ImageStream")));
            this.menuIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.menuIcons.Images.SetKeyName(0, "");
            this.menuIcons.Images.SetKeyName(1, "");
            this.menuIcons.Images.SetKeyName(2, "");
            this.menuIcons.Images.SetKeyName(3, "");
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.ImageIndex = 0;
            this.menuExit.ImageList = this.menuIcons;
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // menuExport
            // 
            this.menuExport.Description = "MenuItem";
            this.menuExport.ImageIndex = 3;
            this.menuExport.ImageList = this.menuIcons;
            this.menuExport.Text = "&Export";
            this.menuExport.Click += new System.EventHandler(this.menuExport_Click);
            // 
            // menuMatrix
            // 
            this.menuMatrix.Description = "MenuItem";
            this.menuMatrix.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuImport,
            this.menuExport});
            this.menuMatrix.Text = "&Matrix";
            // 
            // menuImport
            // 
            this.menuImport.Description = "MenuItem";
            this.menuImport.ImageIndex = 2;
            this.menuImport.ImageList = this.menuIcons;
            this.menuImport.Text = "&Import";
            this.menuImport.Click += new System.EventHandler(this.menuImport_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // ScoringbandMatrix
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbMatrix);
            this.Name = "ScoringbandMatrix";
            this.Text = "ScoringBand Matrix";
            this.Load += new System.EventHandler(this.TermsTypeMatrix_Load);
            this.gbMatrix.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgMatrix)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbMatrix;
        private System.Windows.Forms.DataGridView dgMatrix;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox drpCountry;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuSave;
        private System.Windows.Forms.ImageList menuIcons;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private Crownwood.Magic.Menus.MenuCommand menuExport;
        private Crownwood.Magic.Menus.MenuCommand menuMatrix;
        private Crownwood.Magic.Menus.MenuCommand menuImport;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtStartDate;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.ToolTip ttTermsTypeMatrix;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmb_scorecardtype;
    }
}