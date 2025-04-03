namespace STL.PL
{
	partial class AssemblyOptions
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
            this.btnOk = new System.Windows.Forms.Button();
            this.lblContractor = new System.Windows.Forms.Label();
            this.lblCourts = new System.Windows.Forms.Label();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.radioContractor = new System.Windows.Forms.RadioButton();
            this.radioCourts = new System.Windows.Forms.RadioButton();
            this.radioCustomer = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(98, 142);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // lblContractor
            // 
            this.lblContractor.AutoSize = true;
            this.lblContractor.Location = new System.Drawing.Point(70, 98);
            this.lblContractor.Name = "lblContractor";
            this.lblContractor.Size = new System.Drawing.Size(103, 13);
            this.lblContractor.TabIndex = 5;
            this.lblContractor.Text = "Contractor Assembly";
            // 
            // lblCourts
            // 
            this.lblCourts.AutoSize = true;
            this.lblCourts.Location = new System.Drawing.Point(70, 62);
            this.lblCourts.Name = "lblCourts";
            this.lblCourts.Size = new System.Drawing.Size(84, 13);
            this.lblCourts.TabIndex = 4;
            this.lblCourts.Text = "Courts Assembly";
            // 
            // lblCustomer
            // 
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Location = new System.Drawing.Point(70, 28);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(98, 13);
            this.lblCustomer.TabIndex = 3;
            this.lblCustomer.Text = "Customer Assembly";
            // 
            // radioContractor
            // 
            this.radioContractor.AutoSize = true;
            this.radioContractor.Location = new System.Drawing.Point(197, 98);
            this.radioContractor.Name = "radioContractor";
            this.radioContractor.Size = new System.Drawing.Size(14, 13);
            this.radioContractor.TabIndex = 2;
            this.radioContractor.UseVisualStyleBackColor = true;
            // 
            // radioCourts
            // 
            this.radioCourts.AutoSize = true;
            this.radioCourts.Checked = true;
            this.radioCourts.Location = new System.Drawing.Point(197, 62);
            this.radioCourts.Name = "radioCourts";
            this.radioCourts.Size = new System.Drawing.Size(14, 13);
            this.radioCourts.TabIndex = 1;
            this.radioCourts.TabStop = true;
            this.radioCourts.UseVisualStyleBackColor = true;
            // 
            // radioCustomer
            // 
            this.radioCustomer.AutoSize = true;
            this.radioCustomer.Location = new System.Drawing.Point(197, 28);
            this.radioCustomer.Name = "radioCustomer";
            this.radioCustomer.Size = new System.Drawing.Size(14, 13);
            this.radioCustomer.TabIndex = 0;
            this.radioCustomer.UseVisualStyleBackColor = true;
            // 
            // AssemblyOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 177);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblContractor);
            this.Controls.Add(this.lblCourts);
            this.Controls.Add(this.lblCustomer);
            this.Controls.Add(this.radioContractor);
            this.Controls.Add(this.radioCourts);
            this.Controls.Add(this.radioCustomer);
            this.Name = "AssemblyOptions";
            this.Text = "Assembly Options";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.RadioButton radioCustomer;
        private System.Windows.Forms.RadioButton radioCourts;
        private System.Windows.Forms.RadioButton radioContractor;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.Label lblCourts;
        private System.Windows.Forms.Label lblContractor;
        private System.Windows.Forms.Button btnOk;
	}
}