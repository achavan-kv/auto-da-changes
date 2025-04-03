namespace STL.PL
{
    partial class EmployeeInformation
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
            this.tcEmployeeInfo = new Crownwood.Magic.Controls.TabControl();
            this.tpDaHistory = new Crownwood.Magic.Controls.TabPage();
            this.dgvDaHist = new System.Windows.Forms.DataGridView();
            this.tpEmployeeSummary = new Crownwood.Magic.Controls.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label72 = new System.Windows.Forms.Label();
            this.txtReopenOn = new System.Windows.Forms.TextBox();
            this.txtReopenEmpeeNo = new System.Windows.Forms.TextBox();
            this.txtReopenEmpeeName = new System.Windows.Forms.TextBox();
            this.label73 = new System.Windows.Forms.Label();
            this.label70 = new System.Windows.Forms.Label();
            this.txtCreatedByNo = new System.Windows.Forms.TextBox();
            this.txtCreatedByName = new System.Windows.Forms.TextBox();
            this.label71 = new System.Windows.Forms.Label();
            this.txtCreatedDate = new System.Windows.Forms.TextBox();
            this.label60 = new System.Windows.Forms.Label();
            this.txtSoldByEmpeeNo = new System.Windows.Forms.TextBox();
            this.txtSoldBYEmpeeName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSoldOn = new System.Windows.Forms.TextBox();
            this.label61 = new System.Windows.Forms.Label();
            this.label58 = new System.Windows.Forms.Label();
            this.label59 = new System.Windows.Forms.Label();
            this.txtLastChangedOn = new System.Windows.Forms.TextBox();
            this.txtLstChgEmpeeNo = new System.Windows.Forms.TextBox();
            this.txtLstChgEmpeeName = new System.Windows.Forms.TextBox();
            this.txtDAedOn = new System.Windows.Forms.TextBox();
            this.txtDAbyEmpeeName = new System.Windows.Forms.TextBox();
            this.txtDAEmpeeNo = new System.Windows.Forms.TextBox();
            this.label62 = new System.Windows.Forms.Label();
            this.tcEmployeeInfo.SuspendLayout();
            this.tpDaHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDaHist)).BeginInit();
            this.tpEmployeeSummary.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcEmployeeInfo
            // 
            this.tcEmployeeInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcEmployeeInfo.IDEPixelArea = true;
            this.tcEmployeeInfo.Location = new System.Drawing.Point(0, 0);
            this.tcEmployeeInfo.Name = "tcEmployeeInfo";
            this.tcEmployeeInfo.PositionTop = true;
            this.tcEmployeeInfo.SelectedIndex = 0;
            this.tcEmployeeInfo.SelectedTab = this.tpEmployeeSummary;
            this.tcEmployeeInfo.Size = new System.Drawing.Size(754, 264);
            this.tcEmployeeInfo.TabIndex = 4;
            this.tcEmployeeInfo.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpEmployeeSummary,
            this.tpDaHistory});
            // 
            // tpDaHistory
            // 
            this.tpDaHistory.Controls.Add(this.dgvDaHist);
            this.tpDaHistory.Location = new System.Drawing.Point(0, 25);
            this.tpDaHistory.Name = "tpDaHistory";
            this.tpDaHistory.Selected = false;
            this.tpDaHistory.Size = new System.Drawing.Size(754, 239);
            this.tpDaHistory.TabIndex = 4;
            this.tpDaHistory.Title = "Delivery Authorisation History";
            // 
            // dgvDaHist
            // 
            this.dgvDaHist.AllowUserToAddRows = false;
            this.dgvDaHist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDaHist.Location = new System.Drawing.Point(12, 20);
            this.dgvDaHist.Name = "dgvDaHist";
            this.dgvDaHist.ReadOnly = true;
            this.dgvDaHist.Size = new System.Drawing.Size(380, 173);
            this.dgvDaHist.TabIndex = 0;
            // 
            // tpEmployeeSummary
            // 
            this.tpEmployeeSummary.Controls.Add(this.groupBox2);
            this.tpEmployeeSummary.Location = new System.Drawing.Point(0, 25);
            this.tpEmployeeSummary.Name = "tpEmployeeSummary";
            this.tpEmployeeSummary.Size = new System.Drawing.Size(754, 239);
            this.tpEmployeeSummary.TabIndex = 3;
            this.tpEmployeeSummary.Title = "Employee Summary";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label72);
            this.groupBox2.Controls.Add(this.txtReopenOn);
            this.groupBox2.Controls.Add(this.txtReopenEmpeeNo);
            this.groupBox2.Controls.Add(this.txtReopenEmpeeName);
            this.groupBox2.Controls.Add(this.label73);
            this.groupBox2.Controls.Add(this.label70);
            this.groupBox2.Controls.Add(this.txtCreatedByNo);
            this.groupBox2.Controls.Add(this.txtCreatedByName);
            this.groupBox2.Controls.Add(this.label71);
            this.groupBox2.Controls.Add(this.txtCreatedDate);
            this.groupBox2.Controls.Add(this.label60);
            this.groupBox2.Controls.Add(this.txtSoldByEmpeeNo);
            this.groupBox2.Controls.Add(this.txtSoldBYEmpeeName);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtSoldOn);
            this.groupBox2.Controls.Add(this.label61);
            this.groupBox2.Controls.Add(this.label58);
            this.groupBox2.Controls.Add(this.label59);
            this.groupBox2.Controls.Add(this.txtLastChangedOn);
            this.groupBox2.Controls.Add(this.txtLstChgEmpeeNo);
            this.groupBox2.Controls.Add(this.txtLstChgEmpeeName);
            this.groupBox2.Controls.Add(this.txtDAedOn);
            this.groupBox2.Controls.Add(this.txtDAbyEmpeeName);
            this.groupBox2.Controls.Add(this.txtDAEmpeeNo);
            this.groupBox2.Controls.Add(this.label62);
            this.groupBox2.Location = new System.Drawing.Point(25, 11);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(704, 216);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Employee Summary";
            // 
            // label72
            // 
            this.label72.Location = new System.Drawing.Point(64, 184);
            this.label72.Name = "label72";
            this.label72.Size = new System.Drawing.Size(128, 16);
            this.label72.TabIndex = 22;
            this.label72.Text = "Account Reopened By";
            this.label72.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtReopenOn
            // 
            this.txtReopenOn.Location = new System.Drawing.Point(464, 184);
            this.txtReopenOn.Name = "txtReopenOn";
            this.txtReopenOn.ReadOnly = true;
            this.txtReopenOn.Size = new System.Drawing.Size(100, 23);
            this.txtReopenOn.TabIndex = 24;
            // 
            // txtReopenEmpeeNo
            // 
            this.txtReopenEmpeeNo.Location = new System.Drawing.Point(352, 184);
            this.txtReopenEmpeeNo.Name = "txtReopenEmpeeNo";
            this.txtReopenEmpeeNo.ReadOnly = true;
            this.txtReopenEmpeeNo.Size = new System.Drawing.Size(56, 23);
            this.txtReopenEmpeeNo.TabIndex = 25;
            // 
            // txtReopenEmpeeName
            // 
            this.txtReopenEmpeeName.Location = new System.Drawing.Point(200, 184);
            this.txtReopenEmpeeName.Name = "txtReopenEmpeeName";
            this.txtReopenEmpeeName.ReadOnly = true;
            this.txtReopenEmpeeName.Size = new System.Drawing.Size(136, 23);
            this.txtReopenEmpeeName.TabIndex = 23;
            // 
            // label73
            // 
            this.label73.Location = new System.Drawing.Point(416, 184);
            this.label73.Name = "label73";
            this.label73.Size = new System.Drawing.Size(40, 16);
            this.label73.TabIndex = 26;
            this.label73.Text = " On";
            this.label73.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label70
            // 
            this.label70.Location = new System.Drawing.Point(416, 24);
            this.label70.Name = "label70";
            this.label70.Size = new System.Drawing.Size(40, 16);
            this.label70.TabIndex = 19;
            this.label70.Text = " On";
            this.label70.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCreatedByNo
            // 
            this.txtCreatedByNo.Location = new System.Drawing.Point(352, 24);
            this.txtCreatedByNo.Name = "txtCreatedByNo";
            this.txtCreatedByNo.ReadOnly = true;
            this.txtCreatedByNo.Size = new System.Drawing.Size(56, 23);
            this.txtCreatedByNo.TabIndex = 21;
            // 
            // txtCreatedByName
            // 
            this.txtCreatedByName.Location = new System.Drawing.Point(200, 24);
            this.txtCreatedByName.Name = "txtCreatedByName";
            this.txtCreatedByName.ReadOnly = true;
            this.txtCreatedByName.Size = new System.Drawing.Size(136, 23);
            this.txtCreatedByName.TabIndex = 17;
            // 
            // label71
            // 
            this.label71.Location = new System.Drawing.Point(104, 24);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(88, 16);
            this.label71.TabIndex = 18;
            this.label71.Text = "Created By";
            this.label71.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCreatedDate
            // 
            this.txtCreatedDate.Location = new System.Drawing.Point(464, 24);
            this.txtCreatedDate.Name = "txtCreatedDate";
            this.txtCreatedDate.ReadOnly = true;
            this.txtCreatedDate.Size = new System.Drawing.Size(100, 23);
            this.txtCreatedDate.TabIndex = 20;
            // 
            // label60
            // 
            this.label60.Location = new System.Drawing.Point(416, 64);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(40, 16);
            this.label60.TabIndex = 6;
            this.label60.Text = " On";
            this.label60.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSoldByEmpeeNo
            // 
            this.txtSoldByEmpeeNo.Location = new System.Drawing.Point(352, 64);
            this.txtSoldByEmpeeNo.Name = "txtSoldByEmpeeNo";
            this.txtSoldByEmpeeNo.ReadOnly = true;
            this.txtSoldByEmpeeNo.Size = new System.Drawing.Size(56, 23);
            this.txtSoldByEmpeeNo.TabIndex = 12;
            // 
            // txtSoldBYEmpeeName
            // 
            this.txtSoldBYEmpeeName.Location = new System.Drawing.Point(200, 64);
            this.txtSoldBYEmpeeName.Name = "txtSoldBYEmpeeName";
            this.txtSoldBYEmpeeName.ReadOnly = true;
            this.txtSoldBYEmpeeName.Size = new System.Drawing.Size(136, 23);
            this.txtSoldBYEmpeeName.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(104, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 16);
            this.label4.TabIndex = 1;
            this.label4.Text = "Sold By ";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSoldOn
            // 
            this.txtSoldOn.Location = new System.Drawing.Point(464, 64);
            this.txtSoldOn.Name = "txtSoldOn";
            this.txtSoldOn.ReadOnly = true;
            this.txtSoldOn.Size = new System.Drawing.Size(100, 23);
            this.txtSoldOn.TabIndex = 9;
            // 
            // label61
            // 
            this.label61.Location = new System.Drawing.Point(416, 104);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(40, 16);
            this.label61.TabIndex = 15;
            this.label61.Text = " On";
            this.label61.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label58
            // 
            this.label58.Location = new System.Drawing.Point(96, 104);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(96, 16);
            this.label58.TabIndex = 2;
            this.label58.Text = "D.A\'ed By";
            this.label58.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label59
            // 
            this.label59.Location = new System.Drawing.Point(72, 144);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(120, 16);
            this.label59.TabIndex = 3;
            this.label59.Text = "Last Changed By";
            this.label59.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtLastChangedOn
            // 
            this.txtLastChangedOn.Location = new System.Drawing.Point(464, 144);
            this.txtLastChangedOn.Name = "txtLastChangedOn";
            this.txtLastChangedOn.ReadOnly = true;
            this.txtLastChangedOn.Size = new System.Drawing.Size(100, 23);
            this.txtLastChangedOn.TabIndex = 11;
            // 
            // txtLstChgEmpeeNo
            // 
            this.txtLstChgEmpeeNo.Location = new System.Drawing.Point(352, 144);
            this.txtLstChgEmpeeNo.Name = "txtLstChgEmpeeNo";
            this.txtLstChgEmpeeNo.ReadOnly = true;
            this.txtLstChgEmpeeNo.Size = new System.Drawing.Size(56, 23);
            this.txtLstChgEmpeeNo.TabIndex = 14;
            // 
            // txtLstChgEmpeeName
            // 
            this.txtLstChgEmpeeName.Location = new System.Drawing.Point(200, 144);
            this.txtLstChgEmpeeName.Name = "txtLstChgEmpeeName";
            this.txtLstChgEmpeeName.ReadOnly = true;
            this.txtLstChgEmpeeName.Size = new System.Drawing.Size(136, 23);
            this.txtLstChgEmpeeName.TabIndex = 5;
            // 
            // txtDAedOn
            // 
            this.txtDAedOn.Location = new System.Drawing.Point(464, 104);
            this.txtDAedOn.Name = "txtDAedOn";
            this.txtDAedOn.ReadOnly = true;
            this.txtDAedOn.Size = new System.Drawing.Size(100, 23);
            this.txtDAedOn.TabIndex = 10;
            // 
            // txtDAbyEmpeeName
            // 
            this.txtDAbyEmpeeName.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtDAbyEmpeeName.Location = new System.Drawing.Point(200, 104);
            this.txtDAbyEmpeeName.Name = "txtDAbyEmpeeName";
            this.txtDAbyEmpeeName.ReadOnly = true;
            this.txtDAbyEmpeeName.Size = new System.Drawing.Size(136, 22);
            this.txtDAbyEmpeeName.TabIndex = 4;
            // 
            // txtDAEmpeeNo
            // 
            this.txtDAEmpeeNo.Location = new System.Drawing.Point(352, 104);
            this.txtDAEmpeeNo.Name = "txtDAEmpeeNo";
            this.txtDAEmpeeNo.ReadOnly = true;
            this.txtDAEmpeeNo.Size = new System.Drawing.Size(56, 23);
            this.txtDAEmpeeNo.TabIndex = 13;
            // 
            // label62
            // 
            this.label62.Location = new System.Drawing.Point(416, 144);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(40, 16);
            this.label62.TabIndex = 16;
            this.label62.Text = " On";
            this.label62.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // EmployeeInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 264);
            this.Controls.Add(this.tcEmployeeInfo);
            this.Name = "EmployeeInformation";
            this.Text = "EmployeeInformation";
            this.tcEmployeeInfo.ResumeLayout(false);
            this.tpDaHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDaHist)).EndInit();
            this.tpEmployeeSummary.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Crownwood.Magic.Controls.TabControl tcEmployeeInfo;
        private Crownwood.Magic.Controls.TabPage tpEmployeeSummary;
        private Crownwood.Magic.Controls.TabPage tpDaHistory;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label72;
        private System.Windows.Forms.TextBox txtReopenOn;
        private System.Windows.Forms.TextBox txtReopenEmpeeNo;
        private System.Windows.Forms.TextBox txtReopenEmpeeName;
        private System.Windows.Forms.Label label73;
        private System.Windows.Forms.Label label70;
        private System.Windows.Forms.TextBox txtCreatedByNo;
        private System.Windows.Forms.TextBox txtCreatedByName;
        private System.Windows.Forms.Label label71;
        private System.Windows.Forms.TextBox txtCreatedDate;
        private System.Windows.Forms.Label label60;
        private System.Windows.Forms.TextBox txtSoldByEmpeeNo;
        private System.Windows.Forms.TextBox txtSoldBYEmpeeName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSoldOn;
        private System.Windows.Forms.Label label61;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.Label label59;
        private System.Windows.Forms.TextBox txtLastChangedOn;
        private System.Windows.Forms.TextBox txtLstChgEmpeeNo;
        private System.Windows.Forms.TextBox txtLstChgEmpeeName;
        private System.Windows.Forms.TextBox txtDAedOn;
        private System.Windows.Forms.TextBox txtDAbyEmpeeName;
        private System.Windows.Forms.TextBox txtDAEmpeeNo;
        private System.Windows.Forms.Label label62;
        private System.Windows.Forms.DataGridView dgvDaHist;
    }
}