namespace STL.PL.Collections
{
    partial class LetterMerge
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
            this.gbRunNo = new System.Windows.Forms.GroupBox();
            this.txtRunNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLoadLetters = new System.Windows.Forms.Button();
            this.dgvRunNo = new System.Windows.Forms.DataGridView();
            this.gbLetterCode = new System.Windows.Forms.GroupBox();
            this.chkIncludeGuarantor = new System.Windows.Forms.CheckBox();
            this.chkIncludeSpouse = new System.Windows.Forms.CheckBox();
            this.gbNonCourtsButton = new System.Windows.Forms.GroupBox();
            this.btnNonCourtsGeneratePrint = new System.Windows.Forms.Button();
            this.btnNonCourtsEdit = new System.Windows.Forms.Button();
            this.gbCourtsButton = new System.Windows.Forms.GroupBox();
            this.btnCourtsGeneratePrint = new System.Windows.Forms.Button();
            this.btnCourtsEdit = new System.Windows.Forms.Button();
            this.dgvLetterCodes = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_RunNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_DateStart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_DateFinish = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_Interface = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_Result = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbRunNo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRunNo)).BeginInit();
            this.gbLetterCode.SuspendLayout();
            this.gbNonCourtsButton.SuspendLayout();
            this.gbCourtsButton.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLetterCodes)).BeginInit();
            this.SuspendLayout();
            // 
            // gbRunNo
            // 
            this.gbRunNo.Controls.Add(this.txtRunNo);
            this.gbRunNo.Controls.Add(this.label1);
            this.gbRunNo.Controls.Add(this.btnLoadLetters);
            this.gbRunNo.Controls.Add(this.dgvRunNo);
            this.gbRunNo.Location = new System.Drawing.Point(5, 2);
            this.gbRunNo.Name = "gbRunNo";
            this.gbRunNo.Size = new System.Drawing.Size(681, 191);
            this.gbRunNo.TabIndex = 0;
            this.gbRunNo.TabStop = false;
            this.gbRunNo.Text = "Run No";
            // 
            // txtRunNo
            // 
            this.txtRunNo.Location = new System.Drawing.Point(529, 164);
            this.txtRunNo.Name = "txtRunNo";
            this.txtRunNo.ReadOnly = true;
            this.txtRunNo.Size = new System.Drawing.Size(60, 20);
            this.txtRunNo.TabIndex = 62;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(479, 168);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 61;
            this.label1.Text = "Run No";
            // 
            // btnLoadLetters
            // 
            this.btnLoadLetters.Location = new System.Drawing.Point(595, 162);
            this.btnLoadLetters.Name = "btnLoadLetters";
            this.btnLoadLetters.Size = new System.Drawing.Size(79, 23);
            this.btnLoadLetters.TabIndex = 60;
            this.btnLoadLetters.Text = "Load Letters";
            this.btnLoadLetters.UseVisualStyleBackColor = true;
            this.btnLoadLetters.Click += new System.EventHandler(this.btnLoadLetters_Click);
            // 
            // dgvRunNo
            // 
            this.dgvRunNo.AllowUserToAddRows = false;
            this.dgvRunNo.AllowUserToDeleteRows = false;
            this.dgvRunNo.AllowUserToResizeRows = false;
            this.dgvRunNo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRunNo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.txtColumn_RunNo,
            this.txtColumn_DateStart,
            this.txtColumn_DateFinish,
            this.txtColumn_Interface,
            this.txtColumn_Result});
            this.dgvRunNo.Location = new System.Drawing.Point(6, 19);
            this.dgvRunNo.MultiSelect = false;
            this.dgvRunNo.Name = "dgvRunNo";
            this.dgvRunNo.RowHeadersWidth = 24;
            this.dgvRunNo.Size = new System.Drawing.Size(467, 166);
            this.dgvRunNo.TabIndex = 59;
            this.dgvRunNo.SelectionChanged += new System.EventHandler(this.dgvRunNo_SelectionChanged);
            // 
            // gbLetterCode
            // 
            this.gbLetterCode.Controls.Add(this.chkIncludeGuarantor);
            this.gbLetterCode.Controls.Add(this.chkIncludeSpouse);
            this.gbLetterCode.Controls.Add(this.gbNonCourtsButton);
            this.gbLetterCode.Controls.Add(this.gbCourtsButton);
            this.gbLetterCode.Controls.Add(this.dgvLetterCodes);
            this.gbLetterCode.Location = new System.Drawing.Point(5, 198);
            this.gbLetterCode.Name = "gbLetterCode";
            this.gbLetterCode.Size = new System.Drawing.Size(681, 221);
            this.gbLetterCode.TabIndex = 1;
            this.gbLetterCode.TabStop = false;
            this.gbLetterCode.Text = "Letter Codes";
            // 
            // chkIncludeGuarantor
            // 
            this.chkIncludeGuarantor.AutoSize = true;
            this.chkIncludeGuarantor.Location = new System.Drawing.Point(503, 57);
            this.chkIncludeGuarantor.Name = "chkIncludeGuarantor";
            this.chkIncludeGuarantor.Size = new System.Drawing.Size(141, 17);
            this.chkIncludeGuarantor.TabIndex = 64;
            this.chkIncludeGuarantor.Text = "Include Guarantor Letter";
            this.chkIncludeGuarantor.UseVisualStyleBackColor = true;
            // 
            // chkIncludeSpouse
            // 
            this.chkIncludeSpouse.AutoSize = true;
            this.chkIncludeSpouse.Location = new System.Drawing.Point(503, 34);
            this.chkIncludeSpouse.Name = "chkIncludeSpouse";
            this.chkIncludeSpouse.Size = new System.Drawing.Size(130, 17);
            this.chkIncludeSpouse.TabIndex = 63;
            this.chkIncludeSpouse.Text = "Include Spouse Letter";
            this.chkIncludeSpouse.UseVisualStyleBackColor = true;
            // 
            // gbNonCourtsButton
            // 
            this.gbNonCourtsButton.Controls.Add(this.btnNonCourtsGeneratePrint);
            this.gbNonCourtsButton.Controls.Add(this.btnNonCourtsEdit);
            this.gbNonCourtsButton.Location = new System.Drawing.Point(374, 122);
            this.gbNonCourtsButton.Name = "gbNonCourtsButton";
            this.gbNonCourtsButton.Size = new System.Drawing.Size(123, 80);
            this.gbNonCourtsButton.TabIndex = 62;
            this.gbNonCourtsButton.TabStop = false;
            this.gbNonCourtsButton.Text = "Non-Courts";
            // 
            // btnNonCourtsGeneratePrint
            // 
            this.btnNonCourtsGeneratePrint.Location = new System.Drawing.Point(14, 48);
            this.btnNonCourtsGeneratePrint.Name = "btnNonCourtsGeneratePrint";
            this.btnNonCourtsGeneratePrint.Size = new System.Drawing.Size(95, 23);
            this.btnNonCourtsGeneratePrint.TabIndex = 62;
            this.btnNonCourtsGeneratePrint.Tag = "NonCourts";
            this.btnNonCourtsGeneratePrint.Text = "Generate/Print";
            this.btnNonCourtsGeneratePrint.UseVisualStyleBackColor = true;
            this.btnNonCourtsGeneratePrint.Click += new System.EventHandler(this.btnGeneratePrint_Click);
            // 
            // btnNonCourtsEdit
            // 
            this.btnNonCourtsEdit.Location = new System.Drawing.Point(14, 19);
            this.btnNonCourtsEdit.Name = "btnNonCourtsEdit";
            this.btnNonCourtsEdit.Size = new System.Drawing.Size(95, 23);
            this.btnNonCourtsEdit.TabIndex = 61;
            this.btnNonCourtsEdit.Tag = "NonCourts";
            this.btnNonCourtsEdit.Text = "Edit";
            this.btnNonCourtsEdit.UseVisualStyleBackColor = true;
            this.btnNonCourtsEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // gbCourtsButton
            // 
            this.gbCourtsButton.Controls.Add(this.btnCourtsGeneratePrint);
            this.gbCourtsButton.Controls.Add(this.btnCourtsEdit);
            this.gbCourtsButton.Location = new System.Drawing.Point(374, 25);
            this.gbCourtsButton.Name = "gbCourtsButton";
            this.gbCourtsButton.Size = new System.Drawing.Size(123, 80);
            this.gbCourtsButton.TabIndex = 61;
            this.gbCourtsButton.TabStop = false;
            this.gbCourtsButton.Text = "Courts";
            // 
            // btnCourtsGeneratePrint
            // 
            this.btnCourtsGeneratePrint.Location = new System.Drawing.Point(14, 48);
            this.btnCourtsGeneratePrint.Name = "btnCourtsGeneratePrint";
            this.btnCourtsGeneratePrint.Size = new System.Drawing.Size(95, 23);
            this.btnCourtsGeneratePrint.TabIndex = 62;
            this.btnCourtsGeneratePrint.Tag = "Courts";
            this.btnCourtsGeneratePrint.Text = "Generate/Print";
            this.btnCourtsGeneratePrint.UseVisualStyleBackColor = true;
            this.btnCourtsGeneratePrint.Click += new System.EventHandler(this.btnGeneratePrint_Click);
            // 
            // btnCourtsEdit
            // 
            this.btnCourtsEdit.Location = new System.Drawing.Point(14, 19);
            this.btnCourtsEdit.Name = "btnCourtsEdit";
            this.btnCourtsEdit.Size = new System.Drawing.Size(95, 23);
            this.btnCourtsEdit.TabIndex = 61;
            this.btnCourtsEdit.Tag = "Courts";
            this.btnCourtsEdit.Text = "Edit";
            this.btnCourtsEdit.UseVisualStyleBackColor = true;
            this.btnCourtsEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // dgvLetterCodes
            // 
            this.dgvLetterCodes.AllowUserToAddRows = false;
            this.dgvLetterCodes.AllowUserToDeleteRows = false;
            this.dgvLetterCodes.AllowUserToResizeRows = false;
            this.dgvLetterCodes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLetterCodes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dgvLetterCodes.Location = new System.Drawing.Point(6, 19);
            this.dgvLetterCodes.MultiSelect = false;
            this.dgvLetterCodes.Name = "dgvLetterCodes";
            this.dgvLetterCodes.RowHeadersWidth = 24;
            this.dgvLetterCodes.Size = new System.Drawing.Size(357, 196);
            this.dgvLetterCodes.TabIndex = 60;
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "lettercode";
            this.Column1.HeaderText = "Letter Code";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 120;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "codedescript";
            this.Column2.HeaderText = "Description";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 210;
            // 
            // txtColumn_RunNo
            // 
            this.txtColumn_RunNo.DataPropertyName = "RunNo";
            this.txtColumn_RunNo.HeaderText = "Run No";
            this.txtColumn_RunNo.Name = "txtColumn_RunNo";
            this.txtColumn_RunNo.ReadOnly = true;
            this.txtColumn_RunNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.txtColumn_RunNo.Width = 68;
            // 
            // txtColumn_DateStart
            // 
            this.txtColumn_DateStart.DataPropertyName = "datestart";
            this.txtColumn_DateStart.HeaderText = "Started";
            this.txtColumn_DateStart.Name = "txtColumn_DateStart";
            this.txtColumn_DateStart.ReadOnly = true;
            this.txtColumn_DateStart.Width = 115;
            // 
            // txtColumn_DateFinish
            // 
            this.txtColumn_DateFinish.DataPropertyName = "datefinish";
            this.txtColumn_DateFinish.HeaderText = "Finished";
            this.txtColumn_DateFinish.MinimumWidth = 100;
            this.txtColumn_DateFinish.Name = "txtColumn_DateFinish";
            this.txtColumn_DateFinish.ReadOnly = true;
            this.txtColumn_DateFinish.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.txtColumn_DateFinish.Width = 115;
            // 
            // txtColumn_Interface
            // 
            this.txtColumn_Interface.DataPropertyName = "interface";
            this.txtColumn_Interface.HeaderText = "Interface";
            this.txtColumn_Interface.Name = "txtColumn_Interface";
            this.txtColumn_Interface.ReadOnly = true;
            this.txtColumn_Interface.Width = 80;
            // 
            // txtColumn_Result
            // 
            this.txtColumn_Result.DataPropertyName = "result";
            this.txtColumn_Result.HeaderText = "Result";
            this.txtColumn_Result.Name = "txtColumn_Result";
            this.txtColumn_Result.ReadOnly = true;
            this.txtColumn_Result.Width = 45;
            // 
            // LetterMerge
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 427);
            this.Controls.Add(this.gbRunNo);
            this.Controls.Add(this.gbLetterCode);
            this.Name = "LetterMerge";
            this.Text = "Letter Merge";
            this.Load += new System.EventHandler(this.LetterMerge_Load);
            this.gbRunNo.ResumeLayout(false);
            this.gbRunNo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRunNo)).EndInit();
            this.gbLetterCode.ResumeLayout(false);
            this.gbLetterCode.PerformLayout();
            this.gbNonCourtsButton.ResumeLayout(false);
            this.gbCourtsButton.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLetterCodes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbRunNo;
        private System.Windows.Forms.GroupBox gbLetterCode;
        private System.Windows.Forms.DataGridView dgvRunNo;
        private System.Windows.Forms.TextBox txtRunNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLoadLetters;
        private System.Windows.Forms.GroupBox gbCourtsButton;
        private System.Windows.Forms.Button btnCourtsGeneratePrint;
        private System.Windows.Forms.Button btnCourtsEdit;
        private System.Windows.Forms.DataGridView dgvLetterCodes;
        private System.Windows.Forms.GroupBox gbNonCourtsButton;
        private System.Windows.Forms.Button btnNonCourtsGeneratePrint;
        private System.Windows.Forms.Button btnNonCourtsEdit;
        private System.Windows.Forms.CheckBox chkIncludeGuarantor;
        private System.Windows.Forms.CheckBox chkIncludeSpouse;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_RunNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_DateStart;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_DateFinish;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_Interface;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_Result;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
    }
}