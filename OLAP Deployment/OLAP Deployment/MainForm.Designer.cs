namespace OLAP_Deployment
{
    partial class MainForm
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
            this.dbDefinitionFileLabel = new System.Windows.Forms.Label();
            this.dbDefinitionFileTextBox = new System.Windows.Forms.TextBox();
            this.OLAPSrvConnectionLabel = new System.Windows.Forms.Label();
            this.OLAPSrvConnectionTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.BrowseBtn = new System.Windows.Forms.Button();
            this.SourceDataMartTextBox = new System.Windows.Forms.TextBox();
            this.SourceDataMartLabel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.OkBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.ProcessDbCheckBox = new System.Windows.Forms.CheckBox();
            this.LogTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dbDefinitionFileLabel
            // 
            this.dbDefinitionFileLabel.AutoSize = true;
            this.dbDefinitionFileLabel.Location = new System.Drawing.Point(6, 18);
            this.dbDefinitionFileLabel.Name = "dbDefinitionFileLabel";
            this.dbDefinitionFileLabel.Size = new System.Drawing.Size(116, 17);
            this.dbDefinitionFileLabel.TabIndex = 0;
            this.dbDefinitionFileLabel.Text = "DB Definition File";
            this.dbDefinitionFileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dbDefinitionFileTextBox
            // 
            this.dbDefinitionFileTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dbDefinitionFileTextBox.Location = new System.Drawing.Point(223, 16);
            this.dbDefinitionFileTextBox.Name = "dbDefinitionFileTextBox";
            this.dbDefinitionFileTextBox.Size = new System.Drawing.Size(285, 22);
            this.dbDefinitionFileTextBox.TabIndex = 1;
            // 
            // OLAPSrvConnectionLabel
            // 
            this.OLAPSrvConnectionLabel.AutoSize = true;
            this.OLAPSrvConnectionLabel.Location = new System.Drawing.Point(6, 51);
            this.OLAPSrvConnectionLabel.Name = "OLAPSrvConnectionLabel";
            this.OLAPSrvConnectionLabel.Size = new System.Drawing.Size(207, 17);
            this.OLAPSrvConnectionLabel.TabIndex = 2;
            this.OLAPSrvConnectionLabel.Text = "OLAP Server Connection String";
            // 
            // OLAPSrvConnectionTextBox
            // 
            this.OLAPSrvConnectionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OLAPSrvConnectionTextBox.Location = new System.Drawing.Point(223, 51);
            this.OLAPSrvConnectionTextBox.Multiline = true;
            this.OLAPSrvConnectionTextBox.Name = "OLAPSrvConnectionTextBox";
            this.OLAPSrvConnectionTextBox.Size = new System.Drawing.Size(285, 54);
            this.OLAPSrvConnectionTextBox.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ProcessDbCheckBox);
            this.groupBox1.Controls.Add(this.SourceDataMartTextBox);
            this.groupBox1.Controls.Add(this.SourceDataMartLabel);
            this.groupBox1.Controls.Add(this.BrowseBtn);
            this.groupBox1.Controls.Add(this.OLAPSrvConnectionTextBox);
            this.groupBox1.Controls.Add(this.OLAPSrvConnectionLabel);
            this.groupBox1.Controls.Add(this.dbDefinitionFileTextBox);
            this.groupBox1.Controls.Add(this.dbDefinitionFileLabel);
            this.groupBox1.Location = new System.Drawing.Point(17, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(614, 220);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "DB Definition File|*.asdatabase";
            // 
            // BrowseBtn
            // 
            this.BrowseBtn.Location = new System.Drawing.Point(515, 12);
            this.BrowseBtn.Name = "BrowseBtn";
            this.BrowseBtn.Size = new System.Drawing.Size(75, 30);
            this.BrowseBtn.TabIndex = 4;
            this.BrowseBtn.Text = "Browse";
            this.BrowseBtn.UseVisualStyleBackColor = true;
            this.BrowseBtn.Click += new System.EventHandler(this.BrowseBtn_Click);
            // 
            // SourceDataMartTextBox
            // 
            this.SourceDataMartTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SourceDataMartTextBox.Location = new System.Drawing.Point(223, 118);
            this.SourceDataMartTextBox.Multiline = true;
            this.SourceDataMartTextBox.Name = "SourceDataMartTextBox";
            this.SourceDataMartTextBox.Size = new System.Drawing.Size(285, 54);
            this.SourceDataMartTextBox.TabIndex = 6;
            // 
            // SourceDataMartLabel
            // 
            this.SourceDataMartLabel.AutoSize = true;
            this.SourceDataMartLabel.Location = new System.Drawing.Point(6, 118);
            this.SourceDataMartLabel.Name = "SourceDataMartLabel";
            this.SourceDataMartLabel.Size = new System.Drawing.Size(215, 17);
            this.SourceDataMartLabel.TabIndex = 5;
            this.SourceDataMartLabel.Text = "Source Server Connection String";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.CancelBtn);
            this.groupBox2.Controls.Add(this.OkBtn);
            this.groupBox2.Location = new System.Drawing.Point(17, 235);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(614, 47);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            // 
            // OkBtn
            // 
            this.OkBtn.Location = new System.Drawing.Point(515, 11);
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.Size = new System.Drawing.Size(75, 30);
            this.OkBtn.TabIndex = 3;
            this.OkBtn.Text = "Ok";
            this.OkBtn.UseVisualStyleBackColor = true;
            this.OkBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(433, 11);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 30);
            this.CancelBtn.TabIndex = 4;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // ProcessDbCheckBox
            // 
            this.ProcessDbCheckBox.AutoSize = true;
            this.ProcessDbCheckBox.Checked = true;
            this.ProcessDbCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ProcessDbCheckBox.Location = new System.Drawing.Point(223, 183);
            this.ProcessDbCheckBox.Name = "ProcessDbCheckBox";
            this.ProcessDbCheckBox.Size = new System.Drawing.Size(262, 21);
            this.ProcessDbCheckBox.TabIndex = 5;
            this.ProcessDbCheckBox.Text = "Process the DB after the deployment";
            this.ProcessDbCheckBox.UseVisualStyleBackColor = true;
            // 
            // LogTextBox
            // 
            this.LogTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LogTextBox.Location = new System.Drawing.Point(17, 288);
            this.LogTextBox.Multiline = true;
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LogTextBox.Size = new System.Drawing.Size(614, 103);
            this.LogTextBox.TabIndex = 7;
            // 
            // MainForm
            // 
            this.AcceptButton = this.OkBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.ClientSize = new System.Drawing.Size(651, 413);
            this.ControlBox = false;
            this.Controls.Add(this.LogTextBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "OLAP Deployment Tool";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label OLAPSrvConnectionLabel;
        private System.Windows.Forms.Label dbDefinitionFileLabel;
        private System.Windows.Forms.TextBox dbDefinitionFileTextBox;
        private System.Windows.Forms.TextBox OLAPSrvConnectionTextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button BrowseBtn;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox SourceDataMartTextBox;
        private System.Windows.Forms.Label SourceDataMartLabel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button OkBtn;
        private System.Windows.Forms.CheckBox ProcessDbCheckBox;
        private System.Windows.Forms.TextBox LogTextBox;
    }
}

