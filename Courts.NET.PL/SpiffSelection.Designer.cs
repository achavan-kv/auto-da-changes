namespace STL.PL
{
    partial class SpiffSelection
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpSpiffs = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnReplace = new System.Windows.Forms.Button();
            this.dgvSpiffs = new System.Windows.Forms.DataGridView();
            this.grpSpiffs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSpiffs)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(387, 231);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(56, 24);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpSpiffs
            // 
            this.grpSpiffs.Controls.Add(this.btnOK);
            this.grpSpiffs.Controls.Add(this.btnReplace);
            this.grpSpiffs.Controls.Add(this.dgvSpiffs);
            this.grpSpiffs.Controls.Add(this.btnCancel);
            this.grpSpiffs.Location = new System.Drawing.Point(12, 12);
            this.grpSpiffs.Name = "grpSpiffs";
            this.grpSpiffs.Size = new System.Drawing.Size(644, 265);
            this.grpSpiffs.TabIndex = 10;
            this.grpSpiffs.TabStop = false;
            this.grpSpiffs.Text = "Similar Products With Spiff\'s";
            // 
            // btnOK
            // 
            this.btnOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnOK.Location = new System.Drawing.Point(298, 231);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(56, 24);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "OK";
            this.btnOK.Visible = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnReplace
            // 
            this.btnReplace.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnReplace.Location = new System.Drawing.Point(209, 231);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(56, 24);
            this.btnReplace.TabIndex = 12;
            this.btnReplace.Text = "Replace";
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // dgvSpiffs
            // 
            this.dgvSpiffs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSpiffs.Location = new System.Drawing.Point(20, 25);
            this.dgvSpiffs.Name = "dgvSpiffs";
            this.dgvSpiffs.ReadOnly = true;
            this.dgvSpiffs.Size = new System.Drawing.Size(604, 196);
            this.dgvSpiffs.TabIndex = 11;
            this.dgvSpiffs.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgvSpiffs_MouseUp);
            // 
            // SpiffSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 289);
            this.ControlBox = false;
            this.Controls.Add(this.grpSpiffs);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SpiffSelection";
            this.Text = "Spiff Selection";
            this.grpSpiffs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSpiffs)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpSpiffs;
        private System.Windows.Forms.DataGridView dgvSpiffs;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.Button btnOK;
    }
}