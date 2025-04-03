namespace STL.PL
{
    partial class SummaryUpdateBrokerInterface
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
            this.dgBrokerDetails = new System.Windows.Forms.DataGrid();
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.dataGrid2 = new System.Windows.Forms.DataGrid();
            this.drpBranch = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgBrokerDetails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid2)).BeginInit();
            this.SuspendLayout();
            // 
            // dgBrokerDetails
            // 
            this.dgBrokerDetails.CaptionText = "Broker Run";
            this.dgBrokerDetails.DataMember = "";
            this.dgBrokerDetails.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgBrokerDetails.Location = new System.Drawing.Point(12, 12);
            this.dgBrokerDetails.Name = "dgBrokerDetails";
            this.dgBrokerDetails.ReadOnly = true;
            this.dgBrokerDetails.Size = new System.Drawing.Size(279, 376);
            this.dgBrokerDetails.TabIndex = 28;
            // 
            // dataGrid1
            // 
            this.dataGrid1.CaptionText = "Transaction Totals";
            this.dataGrid1.DataMember = "";
            this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid1.Location = new System.Drawing.Point(316, 12);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.ReadOnly = true;
            this.dataGrid1.Size = new System.Drawing.Size(494, 185);
            this.dataGrid1.TabIndex = 29;
            // 
            // dataGrid2
            // 
            this.dataGrid2.CaptionText = "Transaction Details";
            this.dataGrid2.DataMember = "";
            this.dataGrid2.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid2.Location = new System.Drawing.Point(316, 203);
            this.dataGrid2.Name = "dataGrid2";
            this.dataGrid2.ReadOnly = true;
            this.dataGrid2.Size = new System.Drawing.Size(494, 185);
            this.dataGrid2.TabIndex = 30;
            // 
            // drpBranch
            // 
            this.drpBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranch.Location = new System.Drawing.Point(865, 52);
            this.drpBranch.Name = "drpBranch";
            this.drpBranch.Size = new System.Drawing.Size(48, 21);
            this.drpBranch.TabIndex = 31;
            this.drpBranch.SelectedIndexChanged += new System.EventHandler(this.drpBranch_SelectedIndexChanged);
            // 
            // SummaryUpdateBrokerInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(925, 422);
            this.Controls.Add(this.drpBranch);
            this.Controls.Add(this.dataGrid2);
            this.Controls.Add(this.dataGrid1);
            this.Controls.Add(this.dgBrokerDetails);
            this.Name = "SummaryUpdateBrokerInterface";
            this.Text = "Broker Interface";
            ((System.ComponentModel.ISupportInitialize)(this.dgBrokerDetails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGrid dgBrokerDetails;
        private System.Windows.Forms.DataGrid dataGrid1;
        private System.Windows.Forms.DataGrid dataGrid2;
        private System.Windows.Forms.ComboBox drpBranch;
    }
}