namespace STL.PL
{
   partial class CustomerPhotograph
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
         this.pbPhoto = new System.Windows.Forms.PictureBox();
         this.pbSignature = new System.Windows.Forms.PictureBox();
         ((System.ComponentModel.ISupportInitialize)(this.pbPhoto)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.pbSignature)).BeginInit();
         this.SuspendLayout();
         // 
         // pbPhoto
         // 
         this.pbPhoto.Location = new System.Drawing.Point(82, 24);
         this.pbPhoto.Name = "pbPhoto";
         this.pbPhoto.Size = new System.Drawing.Size(179, 201);
         this.pbPhoto.TabIndex = 0;
         this.pbPhoto.TabStop = false;
         // 
         // pbSignature
         // 
         this.pbSignature.Location = new System.Drawing.Point(4, 256);
         this.pbSignature.Name = "pbSignature";
         this.pbSignature.Size = new System.Drawing.Size(340, 50);
         this.pbSignature.TabIndex = 1;
         this.pbSignature.TabStop = false;
         // 
         // CustomerPhotograph
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(347, 330);
         this.Controls.Add(this.pbSignature);
         this.Controls.Add(this.pbPhoto);
         this.Name = "CustomerPhotograph";
         this.Text = "CustomerPhotograph";
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CustomerPhotograph_FormClosing);
         this.Load += new System.EventHandler(this.CustomerPhotograph_Load);
         ((System.ComponentModel.ISupportInitialize)(this.pbPhoto)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.pbSignature)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.PictureBox pbPhoto;
      private System.Windows.Forms.PictureBox pbSignature;
   }
}