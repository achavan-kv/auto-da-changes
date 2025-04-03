namespace STL.PL.Collections
{
    partial class SpecialArrangementsConsolidated
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
            this.lCustomerName = new System.Windows.Forms.Label();
            this.txtCustId = new System.Windows.Forms.TextBox();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.lCustomerId = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.dgSPAList = new System.Windows.Forms.DataGrid();
            this.label1 = new System.Windows.Forms.Label();
            this.txtArrangementAmt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtExtraAmount = new System.Windows.Forms.TextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgSPAList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // lCustomerName
            // 
            this.lCustomerName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lCustomerName.Location = new System.Drawing.Point(305, 13);
            this.lCustomerName.Name = "lCustomerName";
            this.lCustomerName.Size = new System.Drawing.Size(99, 20);
            this.lCustomerName.TabIndex = 17;
            this.lCustomerName.Text = "Customer Name";
            this.lCustomerName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCustId
            // 
            this.txtCustId.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustId.Location = new System.Drawing.Point(199, 14);
            this.txtCustId.MaxLength = 20;
            this.txtCustId.Name = "txtCustId";
            this.txtCustId.ReadOnly = true;
            this.txtCustId.Size = new System.Drawing.Size(84, 20);
            this.txtCustId.TabIndex = 16;
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerName.Location = new System.Drawing.Point(413, 13);
            this.txtCustomerName.MaxLength = 80;
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(273, 20);
            this.txtCustomerName.TabIndex = 14;
            this.txtCustomerName.TabStop = false;
            // 
            // lCustomerId
            // 
            this.lCustomerId.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lCustomerId.Location = new System.Drawing.Point(72, 13);
            this.lCustomerId.Name = "lCustomerId";
            this.lCustomerId.Size = new System.Drawing.Size(95, 20);
            this.lCustomerId.TabIndex = 15;
            this.lCustomerId.Text = "Customer ID";
            this.lCustomerId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(427, 278);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(57, 23);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(263, 278);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(56, 23);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // dgSPAList
            // 
            this.dgSPAList.CaptionText = "Arrangements List";
            this.dgSPAList.DataMember = "";
            this.dgSPAList.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgSPAList.Location = new System.Drawing.Point(36, 64);
            this.dgSPAList.Name = "dgSPAList";
            this.dgSPAList.RowHeadersVisible = false;
            this.dgSPAList.Size = new System.Drawing.Size(680, 185);
            this.dgSPAList.TabIndex = 18;
            this.dgSPAList.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(73, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Arrangement Instalment";
            // 
            // txtArrangementAmt
            // 
            this.txtArrangementAmt.BackColor = System.Drawing.SystemColors.Control;
            this.txtArrangementAmt.Location = new System.Drawing.Point(199, 36);
            this.txtArrangementAmt.MaxLength = 20;
            this.txtArrangementAmt.Name = "txtArrangementAmt";
            this.txtArrangementAmt.ReadOnly = true;
            this.txtArrangementAmt.Size = new System.Drawing.Size(84, 20);
            this.txtArrangementAmt.TabIndex = 20;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(294, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Odd Payment Amount";
            // 
            // txtExtraAmount
            // 
            this.txtExtraAmount.BackColor = System.Drawing.SystemColors.Control;
            this.txtExtraAmount.Location = new System.Drawing.Point(413, 38);
            this.txtExtraAmount.MaxLength = 20;
            this.txtExtraAmount.Name = "txtExtraAmount";
            this.txtExtraAmount.ReadOnly = true;
            this.txtExtraAmount.Size = new System.Drawing.Size(81, 20);
            this.txtExtraAmount.TabIndex = 22;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // SpecialArrangementsConsolidated
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 324);
            this.ControlBox = false;
            this.Controls.Add(this.txtExtraAmount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtArrangementAmt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgSPAList);
            this.Controls.Add(this.lCustomerName);
            this.Controls.Add(this.txtCustId);
            this.Controls.Add(this.txtCustomerName);
            this.Controls.Add(this.lCustomerId);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Name = "SpecialArrangementsConsolidated";
            this.Text = "Special Arrangements - Consolidated";
            ((System.ComponentModel.ISupportInitialize)(this.dgSPAList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lCustomerName;
        private System.Windows.Forms.TextBox txtCustId;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Label lCustomerId;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.DataGrid dgSPAList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtArrangementAmt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtExtraAmount;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}