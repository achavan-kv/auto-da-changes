namespace STL.PL
{
    partial class FactAuto
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FactAuto));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbEOP = new System.Windows.Forms.RadioButton();
            this.rbEOD = new System.Windows.Forms.RadioButton();
            this.rbEOW = new System.Windows.Forms.RadioButton();
            this.chxZeroStock = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCint = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtProduct = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dtEffectiveDate = new System.Windows.Forms.DateTimePicker();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.chxZeroStock);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtCint);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtProduct);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.dtEffectiveDate);
            this.groupBox1.Location = new System.Drawing.Point(137, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(504, 396);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Fact Auto Control Details";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbEOP);
            this.groupBox2.Controls.Add(this.rbEOD);
            this.groupBox2.Controls.Add(this.rbEOW);
            this.groupBox2.Location = new System.Drawing.Point(216, 180);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(123, 110);
            this.groupBox2.TabIndex = 51;
            this.groupBox2.TabStop = false;
            // 
            // rbEOP
            // 
            this.rbEOP.AutoSize = true;
            this.rbEOP.Location = new System.Drawing.Point(40, 78);
            this.rbEOP.Name = "rbEOP";
            this.rbEOP.Size = new System.Drawing.Size(47, 17);
            this.rbEOP.TabIndex = 49;
            this.rbEOP.Text = "EOP";
            this.rbEOP.UseVisualStyleBackColor = true;
            // 
            // rbEOD
            // 
            this.rbEOD.AutoSize = true;
            this.rbEOD.Checked = true;
            this.rbEOD.Location = new System.Drawing.Point(40, 22);
            this.rbEOD.Name = "rbEOD";
            this.rbEOD.Size = new System.Drawing.Size(48, 17);
            this.rbEOD.TabIndex = 50;
            this.rbEOD.TabStop = true;
            this.rbEOD.Text = "EOD";
            this.rbEOD.UseVisualStyleBackColor = true;
            // 
            // rbEOW
            // 
            this.rbEOW.AutoSize = true;
            this.rbEOW.Location = new System.Drawing.Point(40, 49);
            this.rbEOW.Name = "rbEOW";
            this.rbEOW.Size = new System.Drawing.Size(51, 17);
            this.rbEOW.TabIndex = 48;
            this.rbEOW.Text = "EOW";
            this.rbEOW.UseVisualStyleBackColor = true;
            // 
            // chxZeroStock
            // 
            this.chxZeroStock.AutoSize = true;
            this.chxZeroStock.Location = new System.Drawing.Point(300, 124);
            this.chxZeroStock.Name = "chxZeroStock";
            this.chxZeroStock.Size = new System.Drawing.Size(120, 17);
            this.chxZeroStock.TabIndex = 47;
            this.chxZeroStock.Text = "Exclude Zero Stock";
            this.chxZeroStock.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(101, 333);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 16);
            this.label1.TabIndex = 46;
            this.label1.Text = "Process BMSFCINT";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtCint
            // 
            this.txtCint.Location = new System.Drawing.Point(216, 330);
            this.txtCint.MaxLength = 1;
            this.txtCint.Name = "txtCint";
            this.txtCint.Size = new System.Drawing.Size(44, 20);
            this.txtCint.TabIndex = 45;
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(117, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 16);
            this.label3.TabIndex = 44;
            this.label3.Text = "Full Product";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtProduct
            // 
            this.txtProduct.Location = new System.Drawing.Point(216, 122);
            this.txtProduct.MaxLength = 1;
            this.txtProduct.Name = "txtProduct";
            this.txtProduct.Size = new System.Drawing.Size(44, 20);
            this.txtProduct.TabIndex = 43;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.label4.Location = new System.Drawing.Point(135, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 42;
            this.label4.Text = "Effective Date";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // dtEffectiveDate
            // 
            this.dtEffectiveDate.CustomFormat = "dd MMM yyyy";
            this.dtEffectiveDate.Location = new System.Drawing.Point(216, 62);
            this.dtEffectiveDate.Name = "dtEffectiveDate";
            this.dtEffectiveDate.Size = new System.Drawing.Size(135, 20);
            this.dtEffectiveDate.TabIndex = 41;
            this.dtEffectiveDate.Value = new System.DateTime(2006, 6, 2, 0, 0, 0, 0);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(167, 20);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 55;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSave
            // 
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave.Location = new System.Drawing.Point(301, 20);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 54;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnSave);
            this.groupBox3.Controls.Add(this.btnClear);
            this.groupBox3.Location = new System.Drawing.Point(137, 414);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(504, 56);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataMember = "";
            this.errorProvider1.Icon = ((System.Drawing.Icon)(resources.GetObject("errorProvider1.Icon")));
            // 
            // FactAuto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "FactAuto";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fact Auto Control";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtEffectiveDate;
        private System.Windows.Forms.RadioButton rbEOD;
        private System.Windows.Forms.RadioButton rbEOP;
        private System.Windows.Forms.RadioButton rbEOW;
        private System.Windows.Forms.CheckBox chxZeroStock;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtCint;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox txtProduct;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}