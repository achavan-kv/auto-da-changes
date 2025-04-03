namespace STL.PL
{
    partial class TermsTypeMatrixPopup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TermsTypeMatrixPopup));
            this.drpBand = new System.Windows.Forms.ComboBox();
            this.dgOverview = new System.Windows.Forms.DataGridView();
            this.gbAuthorise = new System.Windows.Forms.GroupBox();
            this.btnAuthorise = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.lLoyaltyTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgOverview)).BeginInit();
            this.gbAuthorise.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // drpBand
            // 
            this.drpBand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBand.Enabled = false;
            this.drpBand.Location = new System.Drawing.Point(222, 25);
            this.drpBand.Name = "drpBand";
            this.drpBand.Size = new System.Drawing.Size(107, 21);
            this.drpBand.TabIndex = 2;
            this.drpBand.SelectionChangeCommitted += new System.EventHandler(this.drpBand_SelectionChangeCommitted);
            this.drpBand.SelectedIndexChanged += new System.EventHandler(this.drpBand_SelectedIndexChanged);
            // 
            // dgOverview
            // 
            this.dgOverview.AllowUserToAddRows = false;
            this.dgOverview.AllowUserToDeleteRows = false;
            this.dgOverview.AllowUserToOrderColumns = true;
            this.dgOverview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgOverview.Location = new System.Drawing.Point(34, 63);
            this.dgOverview.MultiSelect = false;
            this.dgOverview.Name = "dgOverview";
            this.dgOverview.ReadOnly = true;
            this.dgOverview.Size = new System.Drawing.Size(480, 238);
            this.dgOverview.TabIndex = 1;
            this.dgOverview.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgOverview_CellEnter);
            // 
            // gbAuthorise
            // 
            this.gbAuthorise.Controls.Add(this.btnAuthorise);
            this.gbAuthorise.Controls.Add(this.txtPassword);
            this.gbAuthorise.Controls.Add(this.txtUser);
            this.gbAuthorise.Controls.Add(this.label3);
            this.gbAuthorise.Controls.Add(this.label4);
            this.gbAuthorise.Location = new System.Drawing.Point(34, 307);
            this.gbAuthorise.Name = "gbAuthorise";
            this.gbAuthorise.Size = new System.Drawing.Size(480, 76);
            this.gbAuthorise.TabIndex = 11;
            this.gbAuthorise.TabStop = false;
            this.gbAuthorise.Text = "Change Band";
            // 
            // btnAuthorise
            // 
            this.btnAuthorise.Enabled = false;
            this.btnAuthorise.Location = new System.Drawing.Point(364, 37);
            this.btnAuthorise.Name = "btnAuthorise";
            this.btnAuthorise.Size = new System.Drawing.Size(62, 23);
            this.btnAuthorise.TabIndex = 13;
            this.btnAuthorise.Text = "Authorise";
            this.btnAuthorise.Click += new System.EventHandler(this.btnAuthorise_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtPassword.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtPassword.Location = new System.Drawing.Point(188, 40);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(128, 20);
            this.txtPassword.TabIndex = 12;
            this.txtPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPassword_KeyPress);
            this.txtPassword.Validating += new System.ComponentModel.CancelEventHandler(this.txtPassword_Validating);
            // 
            // txtUser
            // 
            this.txtUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtUser.Location = new System.Drawing.Point(18, 40);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(128, 20);
            this.txtUser.TabIndex = 11;
            this.txtUser.Tag = "";
            this.txtUser.Validating += new System.ComponentModel.CancelEventHandler(this.txtUser_Validating);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(188, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 16);
            this.label3.TabIndex = 15;
            this.label3.Text = "Password";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(18, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 16);
            this.label4.TabIndex = 14;
            this.label4.Text = "Employee Number";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(123, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 18);
            this.label1.TabIndex = 16;
            this.label1.Text = "Scoring Band";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(333, 398);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(62, 23);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(156, 398);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(62, 23);
            this.btnOK.TabIndex = 18;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // lLoyaltyTitle
            // 
            this.lLoyaltyTitle.BackColor = System.Drawing.Color.Transparent;
            this.lLoyaltyTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lLoyaltyTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lLoyaltyTitle.Location = new System.Drawing.Point(346, 25);
            this.lLoyaltyTitle.Name = "lLoyaltyTitle";
            this.lLoyaltyTitle.Size = new System.Drawing.Size(168, 18);
            this.lLoyaltyTitle.TabIndex = 19;
            this.lLoyaltyTitle.Text = "[runtime loyalty band title]";
            this.lLoyaltyTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lLoyaltyTitle.Visible = false;
            // 
            // TermsTypeMatrixPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 437);
            this.Controls.Add(this.lLoyaltyTitle);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gbAuthorise);
            this.Controls.Add(this.drpBand);
            this.Controls.Add(this.dgOverview);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TermsTypeMatrixPopup";
            this.Text = "Terms Type Band";
            this.Load += new System.EventHandler(this.TermsTypeMatrixPopup_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgOverview)).EndInit();
            this.gbAuthorise.ResumeLayout(false);
            this.gbAuthorise.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox drpBand;
        private System.Windows.Forms.DataGridView dgOverview;
        private System.Windows.Forms.GroupBox gbAuthorise;
        public System.Windows.Forms.Button btnAuthorise;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label lLoyaltyTitle;
    }
}