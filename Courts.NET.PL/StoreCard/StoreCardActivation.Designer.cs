namespace STL.PL.StoreCard
{
    partial class StoreCardActivate
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btn_activate = new System.Windows.Forms.Button();
            this.txt_SecurityAnswer = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmb_SecurityQ = new System.Windows.Forms.ComboBox();
            this.txt_dob = new System.Windows.Forms.TextBox();
            this.txt_address = new System.Windows.Forms.TextBox();
            this.txt_fullname = new System.Windows.Forms.TextBox();
            this.txt_ProofIdNotes = new System.Windows.Forms.TextBox();
            this.txt_ProofAddressNotes = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.cmb_ProofAddress = new System.Windows.Forms.ComboBox();
            this.cmb_ProofId = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_ActivatedOn = new System.Windows.Forms.TextBox();
            this.ErrorP = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ErrorP)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_activate
            // 
            this.btn_activate.Location = new System.Drawing.Point(657, 346);
            this.btn_activate.Name = "btn_activate";
            this.btn_activate.Size = new System.Drawing.Size(75, 23);
            this.btn_activate.TabIndex = 105;
            this.btn_activate.Text = "Activate";
            this.btn_activate.UseVisualStyleBackColor = true;
            this.btn_activate.Click += new System.EventHandler(this.btn_activate_Click);
            // 
            // txt_SecurityAnswer
            // 
            this.txt_SecurityAnswer.Location = new System.Drawing.Point(301, 122);
            this.txt_SecurityAnswer.MaxLength = 32;
            this.txt_SecurityAnswer.Name = "txt_SecurityAnswer";
            this.txt_SecurityAnswer.Size = new System.Drawing.Size(431, 20);
            this.txt_SecurityAnswer.TabIndex = 104;
            this.txt_SecurityAnswer.TextChanged += new System.EventHandler(this.txt_SecurityAnswer_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(298, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 103;
            this.label3.Text = "Security Answer";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 102;
            this.label2.Text = "Security Question";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmb_SecurityQ
            // 
            this.cmb_SecurityQ.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_SecurityQ.FormattingEnabled = true;
            this.cmb_SecurityQ.Location = new System.Drawing.Point(6, 119);
            this.cmb_SecurityQ.Name = "cmb_SecurityQ";
            this.cmb_SecurityQ.Size = new System.Drawing.Size(253, 21);
            this.cmb_SecurityQ.TabIndex = 101;
            this.cmb_SecurityQ.SelectedIndexChanged += new System.EventHandler(this.cmb_SecurityQ_SelectedIndexChanged);
            // 
            // txt_dob
            // 
            this.txt_dob.Location = new System.Drawing.Point(159, 61);
            this.txt_dob.Name = "txt_dob";
            this.txt_dob.ReadOnly = true;
            this.txt_dob.Size = new System.Drawing.Size(100, 20);
            this.txt_dob.TabIndex = 100;
            // 
            // txt_address
            // 
            this.txt_address.Location = new System.Drawing.Point(301, 26);
            this.txt_address.Multiline = true;
            this.txt_address.Name = "txt_address";
            this.txt_address.ReadOnly = true;
            this.txt_address.Size = new System.Drawing.Size(407, 70);
            this.txt_address.TabIndex = 99;
            // 
            // txt_fullname
            // 
            this.txt_fullname.Location = new System.Drawing.Point(6, 26);
            this.txt_fullname.Name = "txt_fullname";
            this.txt_fullname.ReadOnly = true;
            this.txt_fullname.Size = new System.Drawing.Size(253, 20);
            this.txt_fullname.TabIndex = 98;
            // 
            // txt_ProofIdNotes
            // 
            this.txt_ProofIdNotes.Location = new System.Drawing.Point(301, 176);
            this.txt_ProofIdNotes.MaxLength = 350;
            this.txt_ProofIdNotes.Multiline = true;
            this.txt_ProofIdNotes.Name = "txt_ProofIdNotes";
            this.txt_ProofIdNotes.Size = new System.Drawing.Size(431, 51);
            this.txt_ProofIdNotes.TabIndex = 91;
            // 
            // txt_ProofAddressNotes
            // 
            this.txt_ProofAddressNotes.Location = new System.Drawing.Point(301, 274);
            this.txt_ProofAddressNotes.MaxLength = 350;
            this.txt_ProofAddressNotes.Multiline = true;
            this.txt_ProofAddressNotes.Name = "txt_ProofAddressNotes";
            this.txt_ProofAddressNotes.Size = new System.Drawing.Size(431, 61);
            this.txt_ProofAddressNotes.TabIndex = 95;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(298, 255);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(88, 16);
            this.label12.TabIndex = 97;
            this.label12.Text = "Address Notes:";
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(298, 157);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(64, 16);
            this.label14.TabIndex = 96;
            this.label14.Text = "ID Notes:";
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(3, 255);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(100, 16);
            this.label16.TabIndex = 94;
            this.label16.Text = "Proof of Address:";
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(3, 157);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(64, 16);
            this.label17.TabIndex = 92;
            this.label17.Text = "Proof of ID:";
            // 
            // cmb_ProofAddress
            // 
            this.cmb_ProofAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_ProofAddress.Location = new System.Drawing.Point(3, 271);
            this.cmb_ProofAddress.Name = "cmb_ProofAddress";
            this.cmb_ProofAddress.Size = new System.Drawing.Size(256, 21);
            this.cmb_ProofAddress.TabIndex = 93;
            this.cmb_ProofAddress.SelectedIndexChanged += new System.EventHandler(this.cmb_ProofAddress_SelectedIndexChanged);
            // 
            // cmb_ProofId
            // 
            this.cmb_ProofId.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_ProofId.Location = new System.Drawing.Point(6, 176);
            this.cmb_ProofId.Name = "cmb_ProofId";
            this.cmb_ProofId.Size = new System.Drawing.Size(256, 21);
            this.cmb_ProofId.TabIndex = 90;
            this.cmb_ProofId.SelectedIndexChanged += new System.EventHandler(this.cmb_ProofId_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 64);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(66, 13);
            this.label11.TabIndex = 89;
            this.label11.Text = "Date of Birth";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(298, 10);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(45, 13);
            this.label10.TabIndex = 88;
            this.label10.Text = "Address";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 10);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 13);
            this.label9.TabIndex = 87;
            this.label9.Text = "Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 331);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 106;
            this.label1.Text = "Activated on";
            // 
            // txt_ActivatedOn
            // 
            this.txt_ActivatedOn.Enabled = false;
            this.txt_ActivatedOn.Location = new System.Drawing.Point(101, 328);
            this.txt_ActivatedOn.Name = "txt_ActivatedOn";
            this.txt_ActivatedOn.ReadOnly = true;
            this.txt_ActivatedOn.Size = new System.Drawing.Size(158, 20);
            this.txt_ActivatedOn.TabIndex = 107;
            // 
            // ErrorP
            // 
            this.ErrorP.ContainerControl = this;
            // 
            // StoreCardActivate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_ActivatedOn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_activate);
            this.Controls.Add(this.txt_SecurityAnswer);
            this.Controls.Add(this.cmb_SecurityQ);
            this.Controls.Add(this.txt_dob);
            this.Controls.Add(this.txt_address);
            this.Controls.Add(this.txt_fullname);
            this.Controls.Add(this.txt_ProofIdNotes);
            this.Controls.Add(this.txt_ProofAddressNotes);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.cmb_ProofAddress);
            this.Controls.Add(this.cmb_ProofId);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label3);
            this.Name = "StoreCardActivate";
            this.Size = new System.Drawing.Size(785, 382);
            ((System.ComponentModel.ISupportInitialize)(this.ErrorP)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_activate;
        private System.Windows.Forms.TextBox txt_SecurityAnswer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmb_SecurityQ;
        private System.Windows.Forms.TextBox txt_dob;
        private System.Windows.Forms.TextBox txt_address;
        private System.Windows.Forms.TextBox txt_fullname;
        private System.Windows.Forms.TextBox txt_ProofIdNotes;
        private System.Windows.Forms.TextBox txt_ProofAddressNotes;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox cmb_ProofAddress;
        private System.Windows.Forms.ComboBox cmb_ProofId;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_ActivatedOn;
        private System.Windows.Forms.ErrorProvider ErrorP;
    }
}
