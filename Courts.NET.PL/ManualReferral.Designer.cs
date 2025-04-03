namespace STL.PL
{
    partial class ManualReferral
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
            this.lExplanation = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.referralNotes = new System.Windows.Forms.RichTextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // lExplanation
            // 
            this.lExplanation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lExplanation.Location = new System.Drawing.Point(78, 27);
            this.lExplanation.Name = "lExplanation";
            this.lExplanation.Size = new System.Drawing.Size(214, 20);
            this.lExplanation.TabIndex = 45;
            this.lExplanation.Text = "Please enter the reasons for referral";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(81, 177);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(48, 24);
            this.btnSave.TabIndex = 44;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // referralNotes
            // 
            this.referralNotes.Location = new System.Drawing.Point(32, 72);
            this.referralNotes.MaxLength = 1000;
            this.referralNotes.Name = "referralNotes";
            this.referralNotes.Size = new System.Drawing.Size(312, 80);
            this.referralNotes.TabIndex = 42;
            this.referralNotes.Text = "";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(235, 177);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(48, 24);
            this.btnCancel.TabIndex = 46;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ManualReferral
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 229);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lExplanation);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.referralNotes);
            this.Name = "ManualReferral";
            this.Text = "ManualReferral";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.RichTextBox referralNotes;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lExplanation;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button btnCancel;
    }
}