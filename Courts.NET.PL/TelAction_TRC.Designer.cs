namespace STL.PL
{
    partial class TelAction_TRC
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TelAction_TRC));
            this.gbMain = new System.Windows.Forms.GroupBox();
            this.dtpInitiatedDate_Hider = new System.Windows.Forms.Label();
            this.chkResolved = new System.Windows.Forms.CheckBox();
            this.dtpInitiatedDate = new System.Windows.Forms.DateTimePicker();
            this.txtUserNotes = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.gbButton = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.errProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.gbMain.SuspendLayout();
            this.gbButton.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // gbMain
            // 
            this.gbMain.Controls.Add(this.dtpInitiatedDate_Hider);
            this.gbMain.Controls.Add(this.chkResolved);
            this.gbMain.Controls.Add(this.dtpInitiatedDate);
            this.gbMain.Controls.Add(this.txtUserNotes);
            this.gbMain.Controls.Add(this.label13);
            this.gbMain.Controls.Add(this.label1);
            this.gbMain.Location = new System.Drawing.Point(2, -4);
            this.gbMain.Name = "gbMain";
            this.gbMain.Size = new System.Drawing.Size(444, 114);
            this.gbMain.TabIndex = 4;
            this.gbMain.TabStop = false;
            // 
            // dtpInitiatedDate_Hider
            // 
            this.dtpInitiatedDate_Hider.BackColor = System.Drawing.SystemColors.Window;
            this.dtpInitiatedDate_Hider.Location = new System.Drawing.Point(110, 15);
            this.dtpInitiatedDate_Hider.Name = "dtpInitiatedDate_Hider";
            this.dtpInitiatedDate_Hider.Size = new System.Drawing.Size(105, 14);
            this.dtpInitiatedDate_Hider.TabIndex = 5;
            // 
            // chkResolved
            // 
            this.chkResolved.AutoSize = true;
            this.chkResolved.Location = new System.Drawing.Point(108, 93);
            this.chkResolved.Name = "chkResolved";
            this.chkResolved.Size = new System.Drawing.Size(102, 17);
            this.chkResolved.TabIndex = 0;
            this.chkResolved.Text = "Trace Resolved";
            this.chkResolved.UseVisualStyleBackColor = true;
            // 
            // dtpInitiatedDate
            // 
            this.dtpInitiatedDate.CustomFormat = "ddd dd MMM yyyy";
            this.dtpInitiatedDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpInitiatedDate.Location = new System.Drawing.Point(108, 12);
            this.dtpInitiatedDate.Name = "dtpInitiatedDate";
            this.dtpInitiatedDate.Size = new System.Drawing.Size(126, 20);
            this.dtpInitiatedDate.TabIndex = 4;
            this.dtpInitiatedDate.ValueChanged += new System.EventHandler(this.dtpInitiatedDate_ValueChanged);
            this.dtpInitiatedDate.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dateTimePicker_KeyUp);
            this.dtpInitiatedDate.Enter += new System.EventHandler(this.dtpInitiatedDate_Enter);
            // 
            // txtUserNotes
            // 
            this.txtUserNotes.Location = new System.Drawing.Point(108, 35);
            this.txtUserNotes.Multiline = true;
            this.txtUserNotes.Name = "txtUserNotes";
            this.txtUserNotes.Size = new System.Drawing.Size(314, 54);
            this.txtUserNotes.TabIndex = 33;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(2, 36);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(102, 20);
            this.label13.TabIndex = 32;
            this.label13.Text = "User Notes";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(2, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Initiated Date";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gbButton
            // 
            this.gbButton.Controls.Add(this.btnSave);
            this.gbButton.Controls.Add(this.btnCancel);
            this.gbButton.Location = new System.Drawing.Point(2, 106);
            this.gbButton.Name = "gbButton";
            this.gbButton.Size = new System.Drawing.Size(444, 38);
            this.gbButton.TabIndex = 5;
            this.gbButton.TabStop = false;
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Location = new System.Drawing.Point(300, 11);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 83;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(364, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(74, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // errProvider
            // 
            this.errProvider.ContainerControl = this;
            // 
            // TelAction_TRC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 145);
            this.Controls.Add(this.gbMain);
            this.Controls.Add(this.gbButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TelAction_TRC";
            this.Text = "Trace Details";
            this.Load += new System.EventHandler(this.TelAction_TRC_Load);
            this.gbMain.ResumeLayout(false);
            this.gbMain.PerformLayout();
            this.gbButton.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbMain;
        private System.Windows.Forms.Label dtpInitiatedDate_Hider;
        private System.Windows.Forms.CheckBox chkResolved;
        private System.Windows.Forms.DateTimePicker dtpInitiatedDate;
        private System.Windows.Forms.TextBox txtUserNotes;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbButton;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ErrorProvider errProvider;
        
    }
}