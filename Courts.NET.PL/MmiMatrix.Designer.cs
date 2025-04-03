namespace STL.PL
{
    partial class MmiMatrix
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MmiMatrix));
            this.gbMmiMatrix = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.dgMmiMatrix = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.menuIcons = new System.Windows.Forms.ImageList(this.components);
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.ttMmiMatrix = new System.Windows.Forms.ToolTip(this.components);
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.allowEdit = new System.Windows.Forms.Control();
            this.gbMmiMatrix.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgMmiMatrix)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // gbMmiMatrix
            // 
            this.gbMmiMatrix.BackColor = System.Drawing.SystemColors.Control;
            this.gbMmiMatrix.Controls.Add(this.btnSave);
            this.gbMmiMatrix.Controls.Add(this.dgMmiMatrix);
            this.gbMmiMatrix.Controls.Add(this.label1);
            this.gbMmiMatrix.Location = new System.Drawing.Point(0, 0);
            this.gbMmiMatrix.Name = "gbMmiMatrix";
            this.gbMmiMatrix.Size = new System.Drawing.Size(793, 479);
            this.gbMmiMatrix.TabIndex = 0;
            this.gbMmiMatrix.TabStop = false;
            this.gbMmiMatrix.Text = "Max Monthly Instalment Matrix";
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Location = new System.Drawing.Point(723, 28);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 37;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dgMmiMatrix
            // 
            this.dgMmiMatrix.AllowUserToAddRows = false;
            this.dgMmiMatrix.AllowUserToDeleteRows = false;
            this.dgMmiMatrix.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgMmiMatrix.Location = new System.Drawing.Point(170, 53);
            this.dgMmiMatrix.Name = "dgMmiMatrix";
            this.dgMmiMatrix.Size = new System.Drawing.Size(441, 290);
            this.dgMmiMatrix.TabIndex = 0;
            this.dgMmiMatrix.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgMmiMatrix_CellEndEdit);
            this.dgMmiMatrix.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgMmiMatrix_EditingControlShowing);
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
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuExit});
            this.menuFile.Text = "&File";
            // 
            // allowEdit
            // 
            this.allowEdit.Enabled = false;
            this.allowEdit.Location = new System.Drawing.Point(0, 0);
            this.allowEdit.Name = "allowEdit";
            this.allowEdit.Size = new System.Drawing.Size(0, 0);
            this.allowEdit.TabIndex = 0;
            // 
            // MmiMatrix
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.gbMmiMatrix);
            this.Name = "MmiMatrix";
            this.Text = "Mmi Matrix";
            this.Load += new System.EventHandler(this.MmiMatrix_Load);
            this.gbMmiMatrix.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgMmiMatrix)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbMmiMatrix;
        private System.Windows.Forms.DataGridView dgMmiMatrix;
        private System.Windows.Forms.ImageList menuIcons;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.ToolTip ttMmiMatrix;
        private System.Windows.Forms.Button btnSave;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private System.Windows.Forms.Control allowEdit = null;
    }
}