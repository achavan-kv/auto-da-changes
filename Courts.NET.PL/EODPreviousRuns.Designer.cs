namespace STL.PL
{
    partial class EODPreviousRuns
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
            this.dgPrevious = new System.Windows.Forms.DataGrid();
            this.dgErrors = new System.Windows.Forms.DataGrid();
            this.txtError = new System.Windows.Forms.RichTextBox();
            this.lDescription = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgPrevious)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgErrors)).BeginInit();
            this.SuspendLayout();
            // 
            // dgPrevious
            // 
            this.dgPrevious.CaptionText = "Previous 10 Runs";
            this.dgPrevious.DataMember = "";
            this.dgPrevious.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgPrevious.Location = new System.Drawing.Point(150, 44);
            this.dgPrevious.Name = "dgPrevious";
            this.dgPrevious.ReadOnly = true;
            this.dgPrevious.Size = new System.Drawing.Size(507, 197);
            this.dgPrevious.TabIndex = 1;
            this.dgPrevious.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgPrevious_MouseUp);
            // 
            // dgErrors
            // 
            this.dgErrors.CaptionText = "Errors/Warnings";
            this.dgErrors.DataMember = "";
            this.dgErrors.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgErrors.Location = new System.Drawing.Point(150, 254);
            this.dgErrors.Name = "dgErrors";
            this.dgErrors.ReadOnly = true;
            this.dgErrors.Size = new System.Drawing.Size(507, 101);
            this.dgErrors.TabIndex = 2;
            this.dgErrors.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgErrors_MouseUp);
            // 
            // txtError
            // 
            this.txtError.Location = new System.Drawing.Point(150, 369);
            this.txtError.Name = "txtError";
            this.txtError.ReadOnly = true;
            this.txtError.Size = new System.Drawing.Size(505, 93);
            this.txtError.TabIndex = 3;
            this.txtError.Text = "";
            // 
            // lDescription
            // 
            this.lDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lDescription.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lDescription.Location = new System.Drawing.Point(150, 7);
            this.lDescription.Name = "lDescription";
            this.lDescription.Size = new System.Drawing.Size(505, 28);
            this.lDescription.TabIndex = 9;
            this.lDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // EODPreviousRuns
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.ControlBox = false;
            this.Controls.Add(this.lDescription);
            this.Controls.Add(this.txtError);
            this.Controls.Add(this.dgErrors);
            this.Controls.Add(this.dgPrevious);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EODPreviousRuns";
            this.Text = "Previous Runs";
            ((System.ComponentModel.ISupportInitialize)(this.dgPrevious)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgErrors)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGrid dgPrevious;
        private System.Windows.Forms.DataGrid dgErrors;
        private System.Windows.Forms.RichTextBox txtError;
        private System.Windows.Forms.Label lDescription;
    }
}