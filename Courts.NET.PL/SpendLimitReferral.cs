using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace STL.PL
{
	/// <summary>
	/// Popup prompt to warn the user when a customer credit limit has been
	/// exceeded. The user has the option to refer the account.
	/// </summary>
	public class SpendLimitReferral : CommonForm
	{
		public System.Windows.Forms.RichTextBox rtxtNewReferralNotes;
		private System.Windows.Forms.Label lExplanation;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnRefer;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
      private bool m_refer = false;
      public bool refer
      {
         get
         {
            return m_refer;
         }
         set
         {
            m_refer = value;
         }
      }

		public SpendLimitReferral(TranslationDummy d)
		{
			InitializeComponent();
		}

		public SpendLimitReferral()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.rtxtNewReferralNotes = new System.Windows.Forms.RichTextBox();
			this.lExplanation = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnRefer = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// rtxtNewReferralNotes
			// 
			this.rtxtNewReferralNotes.Location = new System.Drawing.Point(32, 72);
			this.rtxtNewReferralNotes.MaxLength = 1000;
			this.rtxtNewReferralNotes.Name = "rtxtNewReferralNotes";
			this.rtxtNewReferralNotes.Size = new System.Drawing.Size(312, 80);
			this.rtxtNewReferralNotes.TabIndex = 41;
			this.rtxtNewReferralNotes.Text = "";
			// 
			// lExplanation
			// 
			this.lExplanation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lExplanation.Location = new System.Drawing.Point(40, 8);
			this.lExplanation.Name = "lExplanation";
			this.lExplanation.Size = new System.Drawing.Size(304, 40);
			this.lExplanation.TabIndex = 42;
			this.lExplanation.Text = "RF Credit Limit Exceeded.  Do you want to Manually Refer?";
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(200, 176);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(48, 24);
			this.btnCancel.TabIndex = 44;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnRefer
			// 
			this.btnRefer.Location = new System.Drawing.Point(128, 176);
			this.btnRefer.Name = "btnRefer";
			this.btnRefer.Size = new System.Drawing.Size(48, 24);
			this.btnRefer.TabIndex = 43;
			this.btnRefer.Text = "Refer";
			this.btnRefer.Click += new System.EventHandler(this.btnRefer_Click);
			// 
			// SpendLimitReferral
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(384, 229);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btnCancel,
																		  this.btnRefer,
																		  this.lExplanation,
																		  this.rtxtNewReferralNotes});
			this.Name = "SpendLimitReferral";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SpendLimitReferral";
			this.ResumeLayout(false);

		}
		#endregion

		private void btnRefer_Click(object sender, System.EventArgs e)
		{
			refer = true;
			Close();
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			refer = false;
			Close();
		}
	}
}
