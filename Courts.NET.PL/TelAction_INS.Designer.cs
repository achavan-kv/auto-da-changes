namespace STL.PL
{
    partial class TelAction_INS
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TelAction_INS));
            this.gbMain = new System.Windows.Forms.GroupBox();
            this.cmbClaimType = new System.Windows.Forms.ComboBox();
            this.cmbInsType = new System.Windows.Forms.ComboBox();
            this.dtpInitiatedDate_Hider = new System.Windows.Forms.Label();
            this.chkInsApproved = new System.Windows.Forms.CheckBox();
            this.dtpInitiatedDate = new System.Windows.Forms.DateTimePicker();
            this.txtUserNotes = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtInsAmount = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
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
            this.gbMain.Controls.Add(this.cmbClaimType);
            this.gbMain.Controls.Add(this.cmbInsType);
            this.gbMain.Controls.Add(this.dtpInitiatedDate_Hider);
            this.gbMain.Controls.Add(this.chkInsApproved);
            this.gbMain.Controls.Add(this.dtpInitiatedDate);
            this.gbMain.Controls.Add(this.txtUserNotes);
            this.gbMain.Controls.Add(this.label13);
            this.gbMain.Controls.Add(this.label8);
            this.gbMain.Controls.Add(this.txtInsAmount);
            this.gbMain.Controls.Add(this.label3);
            this.gbMain.Controls.Add(this.label2);
            this.gbMain.Controls.Add(this.label1);
            this.gbMain.Location = new System.Drawing.Point(2, -4);
            this.gbMain.Name = "gbMain";
            this.gbMain.Size = new System.Drawing.Size(444, 185);
            this.gbMain.TabIndex = 1;
            this.gbMain.TabStop = false;
            // 
            // cmbClaimType
            // 
            this.cmbClaimType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClaimType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbClaimType.FormattingEnabled = true;
            this.cmbClaimType.Location = new System.Drawing.Point(108, 82);
            this.cmbClaimType.Name = "cmbClaimType";
            this.cmbClaimType.Size = new System.Drawing.Size(126, 21);
            this.cmbClaimType.TabIndex = 35;
            // 
            // cmbInsType
            // 
            this.cmbInsType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInsType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbInsType.FormattingEnabled = true;
            this.cmbInsType.Location = new System.Drawing.Point(108, 12);
            this.cmbInsType.Name = "cmbInsType";
            this.cmbInsType.Size = new System.Drawing.Size(126, 21);
            this.cmbInsType.TabIndex = 34;
            // 
            // dtpInitiatedDate_Hider
            // 
            this.dtpInitiatedDate_Hider.BackColor = System.Drawing.SystemColors.Window;
            this.dtpInitiatedDate_Hider.Location = new System.Drawing.Point(110, 39);
            this.dtpInitiatedDate_Hider.Name = "dtpInitiatedDate_Hider";
            this.dtpInitiatedDate_Hider.Size = new System.Drawing.Size(105, 14);
            this.dtpInitiatedDate_Hider.TabIndex = 5;
            // 
            // chkInsApproved
            // 
            this.chkInsApproved.AutoSize = true;
            this.chkInsApproved.Location = new System.Drawing.Point(108, 164);
            this.chkInsApproved.Name = "chkInsApproved";
            this.chkInsApproved.Size = new System.Drawing.Size(122, 17);
            this.chkInsApproved.TabIndex = 0;
            this.chkInsApproved.Text = "Insurance Approved";
            this.chkInsApproved.UseVisualStyleBackColor = true;
            // 
            // dtpInitiatedDate
            // 
            this.dtpInitiatedDate.CustomFormat = "ddd dd MMM yyyy";
            this.dtpInitiatedDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpInitiatedDate.Location = new System.Drawing.Point(108, 36);
            this.dtpInitiatedDate.Name = "dtpInitiatedDate";
            this.dtpInitiatedDate.Size = new System.Drawing.Size(126, 20);
            this.dtpInitiatedDate.TabIndex = 4;
            this.dtpInitiatedDate.ValueChanged += new System.EventHandler(this.dtpInitiatedDate_ValueChanged);
            this.dtpInitiatedDate.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dateTimePicker_KeyUp);
            this.dtpInitiatedDate.Enter += new System.EventHandler(this.dtpInitiatedDate_Enter);
            // 
            // txtUserNotes
            // 
            this.txtUserNotes.Location = new System.Drawing.Point(108, 106);
            this.txtUserNotes.Multiline = true;
            this.txtUserNotes.Name = "txtUserNotes";
            this.txtUserNotes.Size = new System.Drawing.Size(314, 54);
            this.txtUserNotes.TabIndex = 33;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(2, 106);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(102, 20);
            this.label13.TabIndex = 32;
            this.label13.Text = "User Notes";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(2, 82);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(102, 20);
            this.label8.TabIndex = 8;
            this.label8.Text = "Claim";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtInsAmount
            // 
            this.txtInsAmount.Location = new System.Drawing.Point(108, 59);
            this.txtInsAmount.Name = "txtInsAmount";
            this.txtInsAmount.Size = new System.Drawing.Size(126, 20);
            this.txtInsAmount.TabIndex = 7;
            this.txtInsAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(2, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Insurance Amount";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(2, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Initiated Date";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(2, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Insurance Type";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gbButton
            // 
            this.gbButton.Controls.Add(this.btnSave);
            this.gbButton.Controls.Add(this.btnCancel);
            this.gbButton.Location = new System.Drawing.Point(2, 177);
            this.gbButton.Name = "gbButton";
            this.gbButton.Size = new System.Drawing.Size(444, 38);
            this.gbButton.TabIndex = 2;
            this.gbButton.TabStop = false;
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Location = new System.Drawing.Point(310, 11);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 82;
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
            // TelAction_INS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 216);
            this.ControlBox = false;
            this.Controls.Add(this.gbMain);
            this.Controls.Add(this.gbButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.IconFileUrl = "";
            this.KeyPreview = false;
            this.Name = "TelAction_INS";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Insurance Details";
            this.Load += new System.EventHandler(this.TelAction_INS_Load);
            this.gbMain.ResumeLayout(false);
            this.gbMain.PerformLayout();
            this.gbButton.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbMain;
        private System.Windows.Forms.Label dtpInitiatedDate_Hider;
        private System.Windows.Forms.DateTimePicker dtpInitiatedDate;
        private System.Windows.Forms.TextBox txtUserNotes;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtInsAmount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbButton;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbInsType;
        private System.Windows.Forms.ComboBox cmbClaimType;
        private System.Windows.Forms.CheckBox chkInsApproved;
        private System.Windows.Forms.ErrorProvider errProvider;
        private System.Windows.Forms.Button btnSave;
    }
}