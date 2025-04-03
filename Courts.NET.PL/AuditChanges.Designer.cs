namespace STL.PL
{
    partial class AuditChanges
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
            this.tcSummary = new Crownwood.Magic.Controls.TabControl();
            this.tpAgreementAudit = new Crownwood.Magic.Controls.TabPage();
            this.dgAgreement = new System.Windows.Forms.DataGrid();
            this.tpLineItemAudit = new Crownwood.Magic.Controls.TabPage();
            this.dgLineItem = new System.Windows.Forms.DataGrid();
            this.tpInstalPlanAudit = new Crownwood.Magic.Controls.TabPage();
            this.dgInstalPlan = new System.Windows.Forms.DataGrid();
            this.tpDeliveryNotificationAudit = new Crownwood.Magic.Controls.TabPage();
            this.dgDeliveryNotificationAudit = new System.Windows.Forms.DataGrid();
            this.tcSummary.SuspendLayout();
            this.tpAgreementAudit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAgreement)).BeginInit();
            this.tpLineItemAudit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgLineItem)).BeginInit();
            this.tpInstalPlanAudit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgInstalPlan)).BeginInit();
            this.tpDeliveryNotificationAudit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDeliveryNotificationAudit)).BeginInit();
            this.SuspendLayout();
            // 
            // tcSummary
            // 
            this.tcSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcSummary.IDEPixelArea = true;
            this.tcSummary.Location = new System.Drawing.Point(0, 0);
            this.tcSummary.Name = "tcSummary";
            this.tcSummary.PositionTop = true;
            this.tcSummary.SelectedIndex = 3;
            this.tcSummary.SelectedTab = this.tpDeliveryNotificationAudit;
            this.tcSummary.Size = new System.Drawing.Size(754, 264);
            this.tcSummary.TabIndex = 3;
            this.tcSummary.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpLineItemAudit,
            this.tpAgreementAudit,
            this.tpInstalPlanAudit,
            this.tpDeliveryNotificationAudit});
            // 
            // tpAgreementAudit
            // 
            this.tpAgreementAudit.Controls.Add(this.dgAgreement);
            this.tpAgreementAudit.Location = new System.Drawing.Point(0, 25);
            this.tpAgreementAudit.Name = "tpAgreementAudit";
            this.tpAgreementAudit.Selected = false;
            this.tpAgreementAudit.Size = new System.Drawing.Size(754, 239);
            this.tpAgreementAudit.TabIndex = 4;
            this.tpAgreementAudit.Title = "Agreement Audit";
            // 
            // dgAgreement
            // 
            this.dgAgreement.CaptionFont = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
            this.dgAgreement.DataMember = "";
            this.dgAgreement.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAgreement.Location = new System.Drawing.Point(12, 17);
            this.dgAgreement.Name = "dgAgreement";
            this.dgAgreement.ReadOnly = true;
            this.dgAgreement.Size = new System.Drawing.Size(642, 193);
            this.dgAgreement.TabIndex = 2;
            // 
            // tpLineItemAudit
            // 
            this.tpLineItemAudit.Controls.Add(this.dgLineItem);
            this.tpLineItemAudit.Location = new System.Drawing.Point(0, 25);
            this.tpLineItemAudit.Name = "tpLineItemAudit";
            this.tpLineItemAudit.Selected = false;
            this.tpLineItemAudit.Size = new System.Drawing.Size(754, 239);
            this.tpLineItemAudit.TabIndex = 3;
            this.tpLineItemAudit.Title = "Line Item Audit";
            // 
            // dgLineItem
            // 
            this.dgLineItem.CaptionFont = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
            this.dgLineItem.DataMember = "";
            this.dgLineItem.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgLineItem.Location = new System.Drawing.Point(12, 17);
            this.dgLineItem.Name = "dgLineItem";
            this.dgLineItem.ReadOnly = true;
            this.dgLineItem.Size = new System.Drawing.Size(642, 193);
            this.dgLineItem.TabIndex = 1;
            // 
            // tpInstalPlanAudit
            // 
            this.tpInstalPlanAudit.Controls.Add(this.dgInstalPlan);
            this.tpInstalPlanAudit.Location = new System.Drawing.Point(0, 25);
            this.tpInstalPlanAudit.Name = "tpInstalPlanAudit";
            this.tpInstalPlanAudit.Selected = false;
            this.tpInstalPlanAudit.Size = new System.Drawing.Size(754, 239);
            this.tpInstalPlanAudit.TabIndex = 5;
            this.tpInstalPlanAudit.Title = "Instal Plan Audit";
            // 
            // dgInstalPlan
            // 
            this.dgInstalPlan.CaptionFont = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
            this.dgInstalPlan.DataMember = "";
            this.dgInstalPlan.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgInstalPlan.Location = new System.Drawing.Point(12, 17);
            this.dgInstalPlan.Name = "dgInstalPlan";
            this.dgInstalPlan.ReadOnly = true;
            this.dgInstalPlan.Size = new System.Drawing.Size(642, 193);
            this.dgInstalPlan.TabIndex = 3;
            // 
            // tpDeliveryNotificationAudit
            // 
            this.tpDeliveryNotificationAudit.Controls.Add(this.dgDeliveryNotificationAudit);
            this.tpDeliveryNotificationAudit.Location = new System.Drawing.Point(0, 25);
            this.tpDeliveryNotificationAudit.Name = "tpDeliveryNotificationAudit";
            this.tpDeliveryNotificationAudit.Size = new System.Drawing.Size(754, 239);
            this.tpDeliveryNotificationAudit.TabIndex = 6;
            this.tpDeliveryNotificationAudit.Title = "Delivery Notification Audit";
            // 
            // dgDeliveryNotificationAudit
            // 
            this.dgDeliveryNotificationAudit.CaptionFont = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
            this.dgDeliveryNotificationAudit.DataMember = "";
            this.dgDeliveryNotificationAudit.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgDeliveryNotificationAudit.Location = new System.Drawing.Point(12, 17);
            this.dgDeliveryNotificationAudit.Name = "dgDeliveryNotificationAudit";
            this.dgDeliveryNotificationAudit.ReadOnly = true;
            this.dgDeliveryNotificationAudit.Size = new System.Drawing.Size(642, 193);
            this.dgDeliveryNotificationAudit.TabIndex = 4;
            // 
            // AuditChanges
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 264);
            this.Controls.Add(this.tcSummary);
            this.Name = "AuditChanges";
            this.Text = "AuditChanges";
            this.tcSummary.ResumeLayout(false);
            this.tpAgreementAudit.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAgreement)).EndInit();
            this.tpLineItemAudit.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgLineItem)).EndInit();
            this.tpInstalPlanAudit.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgInstalPlan)).EndInit();
            this.tpDeliveryNotificationAudit.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgDeliveryNotificationAudit)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Crownwood.Magic.Controls.TabControl tcSummary;
        private Crownwood.Magic.Controls.TabPage tpLineItemAudit;
        private Crownwood.Magic.Controls.TabPage tpAgreementAudit;
        private Crownwood.Magic.Controls.TabPage tpInstalPlanAudit;
        private Crownwood.Magic.Controls.TabPage tpDeliveryNotificationAudit;
        public System.Windows.Forms.DataGrid dgLineItem;
        public System.Windows.Forms.DataGrid dgAgreement;
        public System.Windows.Forms.DataGrid dgInstalPlan;
        public System.Windows.Forms.DataGrid dgDeliveryNotificationAudit;
    }
}