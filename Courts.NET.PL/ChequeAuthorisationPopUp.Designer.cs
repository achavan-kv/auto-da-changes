namespace STL.PL
{
    partial class ChequeAuthorisationPopUp
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
           this.gbReturnedCheques = new System.Windows.Forms.GroupBox();
           this.dgReturnedCheques1 = new STL.PL.DataGridCellTips();
           this.groupBox1 = new System.Windows.Forms.GroupBox();
           this.txtEmpNo = new System.Windows.Forms.TextBox();
           this.txtPassword = new System.Windows.Forms.TextBox();
           this.label1 = new System.Windows.Forms.Label();
           this.label2 = new System.Windows.Forms.Label();
           this.btnContinue = new System.Windows.Forms.Button();
           this.btnCancel = new System.Windows.Forms.Button();
           this.label3 = new System.Windows.Forms.Label();
           this.gbReturnedCheques.SuspendLayout();
           ((System.ComponentModel.ISupportInitialize)(this.dgReturnedCheques1)).BeginInit();
           this.groupBox1.SuspendLayout();
           this.SuspendLayout();
           // 
           // gbReturnedCheques
           // 
           this.gbReturnedCheques.Controls.Add(this.dgReturnedCheques1);
           this.gbReturnedCheques.Location = new System.Drawing.Point(12, 37);
           this.gbReturnedCheques.Name = "gbReturnedCheques";
           this.gbReturnedCheques.Size = new System.Drawing.Size(405, 118);
           this.gbReturnedCheques.TabIndex = 2;
           this.gbReturnedCheques.TabStop = false;
           this.gbReturnedCheques.Text = "Returned Cheques";
           // 
           // dgReturnedCheques1
           // 
           this.dgReturnedCheques1.AllowNavigation = false;
           this.dgReturnedCheques1.AllowSorting = false;
           this.dgReturnedCheques1.CaptionVisible = false;
           this.dgReturnedCheques1.DataMember = "";
           this.dgReturnedCheques1.Dock = System.Windows.Forms.DockStyle.Fill;
           this.dgReturnedCheques1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
           this.dgReturnedCheques1.Location = new System.Drawing.Point(3, 16);
           this.dgReturnedCheques1.Name = "dgReturnedCheques1";
           this.dgReturnedCheques1.ReadOnly = true;
           this.dgReturnedCheques1.Size = new System.Drawing.Size(399, 99);
           this.dgReturnedCheques1.TabIndex = 7;
           this.dgReturnedCheques1.TabStop = false;
           this.dgReturnedCheques1.ToolTipColumn = 1;
           this.dgReturnedCheques1.ToolTipDelay = 200;
           // 
           // groupBox1
           // 
           this.groupBox1.Controls.Add(this.txtEmpNo);
           this.groupBox1.Controls.Add(this.txtPassword);
           this.groupBox1.Controls.Add(this.label1);
           this.groupBox1.Controls.Add(this.label2);
           this.groupBox1.Location = new System.Drawing.Point(12, 161);
           this.groupBox1.Name = "groupBox1";
           this.groupBox1.Size = new System.Drawing.Size(405, 79);
           this.groupBox1.TabIndex = 3;
           this.groupBox1.TabStop = false;
           this.groupBox1.Text = "Cheque Authorisation User Details";
           // 
           // txtEmpNo
           // 
           this.txtEmpNo.Location = new System.Drawing.Point(9, 45);
           this.txtEmpNo.Name = "txtEmpNo";
           this.txtEmpNo.Size = new System.Drawing.Size(144, 20);
           this.txtEmpNo.TabIndex = 6;
           // 
           // txtPassword
           // 
           this.txtPassword.Location = new System.Drawing.Point(253, 45);
           this.txtPassword.Name = "txtPassword";
           this.txtPassword.Size = new System.Drawing.Size(143, 20);
           this.txtPassword.TabIndex = 7;
           this.txtPassword.UseSystemPasswordChar = true;
           this.txtPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPassword_KeyPress);
           this.txtPassword.Validating += new System.ComponentModel.CancelEventHandler(this.txtPassword_Validating);
           this.txtPassword.TextChanged += new System.EventHandler(this.txtPassword_TextChanged);
           // 
           // label1
           // 
           this.label1.AutoSize = true;
           this.label1.Location = new System.Drawing.Point(6, 29);
           this.label1.Name = "label1";
           this.label1.Size = new System.Drawing.Size(93, 13);
           this.label1.TabIndex = 6;
           this.label1.Text = "Employee Number";
           // 
           // label2
           // 
           this.label2.AutoSize = true;
           this.label2.Location = new System.Drawing.Point(250, 29);
           this.label2.Name = "label2";
           this.label2.Size = new System.Drawing.Size(53, 13);
           this.label2.TabIndex = 7;
           this.label2.Text = "Password";
           // 
           // btnContinue
           // 
           this.btnContinue.Enabled = false;
           this.btnContinue.Location = new System.Drawing.Point(12, 246);
           this.btnContinue.Name = "btnContinue";
           this.btnContinue.Size = new System.Drawing.Size(75, 23);
           this.btnContinue.TabIndex = 8;
           this.btnContinue.Text = "Continue";
           this.btnContinue.UseVisualStyleBackColor = true;
           this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
           // 
           // btnCancel
           // 
           this.btnCancel.Location = new System.Drawing.Point(342, 246);
           this.btnCancel.Name = "btnCancel";
           this.btnCancel.Size = new System.Drawing.Size(75, 23);
           this.btnCancel.TabIndex = 9;
           this.btnCancel.Text = "Cancel";
           this.btnCancel.UseVisualStyleBackColor = true;
           this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
           // 
           // label3
           // 
           this.label3.AutoSize = true;
           this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
           this.label3.Location = new System.Drawing.Point(18, 9);
           this.label3.Name = "label3";
           this.label3.Size = new System.Drawing.Size(380, 13);
           this.label3.TabIndex = 6;
           this.label3.Text = "Returned cheque present on the following accounts belonging to the customer.";
           // 
           // ChequeAuthorisationPopUp
           // 
           this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
           this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
           this.ClientSize = new System.Drawing.Size(432, 288);
           this.Controls.Add(this.gbReturnedCheques);
           this.Controls.Add(this.label3);
           this.Controls.Add(this.btnCancel);
           this.Controls.Add(this.btnContinue);
           this.Controls.Add(this.groupBox1);
           this.MaximizeBox = false;
           this.MaximumSize = new System.Drawing.Size(440, 315);
           this.MinimizeBox = false;
           this.MinimumSize = new System.Drawing.Size(440, 315);
           this.Name = "ChequeAuthorisationPopUp";
           this.ShowInTaskbar = false;
           this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
           this.Text = "Cheque Authorisation";
           this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChequeAuthorisationPopUp_FormClosing);
           this.gbReturnedCheques.ResumeLayout(false);
           ((System.ComponentModel.ISupportInitialize)(this.dgReturnedCheques1)).EndInit();
           this.groupBox1.ResumeLayout(false);
           this.groupBox1.PerformLayout();
           this.ResumeLayout(false);
           this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbReturnedCheques;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtEmpNo;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private DataGridCellTips dgReturnedCheques1;

    }
}