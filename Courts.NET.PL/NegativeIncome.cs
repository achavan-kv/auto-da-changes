using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace STL.PL
{
	/// <summary>
	/// Popup prompt used by credit sanctioning to inform the user that the disposable income
	/// is negative and must be amened before the credit application can proceed.
	/// </summary>
	public class NegativeIncome : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnReview;
		private bool _cancel = false; 
		public bool Cancel 
		{
			get{return _cancel;}
		}
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public NegativeIncome(TranslationDummy t)
		{
			InitializeComponent();
		}

		public NegativeIncome()
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
			this.label1 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnReview = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(208, 32);
			this.label1.TabIndex = 0;
			this.label1.Text = "Monthly disposable income is negative. Please review or cancel the account";
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(104, 56);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(56, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnReview
			// 
			this.btnReview.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnReview.Location = new System.Drawing.Point(40, 56);
			this.btnReview.Name = "btnReview";
			this.btnReview.Size = new System.Drawing.Size(56, 23);
			this.btnReview.TabIndex = 2;
			this.btnReview.Text = "Review";
			this.btnReview.Click += new System.EventHandler(this.btnReview_Click);
			// 
			// NegativeIncome
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnReview;
			this.ClientSize = new System.Drawing.Size(208, 93);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btnReview,
																		  this.btnCancel,
																		  this.label1});
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NegativeIncome";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Negative Income";
			this.ResumeLayout(false);

		}
		#endregion

		private void btnReview_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			_cancel = true;
			Close();
		}
	}
}
