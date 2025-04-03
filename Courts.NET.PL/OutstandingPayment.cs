using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common.Static;
using STL.Common;

namespace STL.PL
{
	/// <summary>
	/// Summary description for OutstandingPayment.
	/// </summary>
	public class OutstandingPayment : CommonForm
	{
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label txtMsg;
		public System.Windows.Forms.RadioButton rbCancel;
		public System.Windows.Forms.RadioButton rbRemain;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public OutstandingPayment(TranslationDummy d)
		{
			InitializeComponent();
		}

		public OutstandingPayment(Form root)
		{
			InitializeComponent();

			FormRoot = root;
			txtMsg.Text = GetResource("M_OUTSTANDINGPAYMENTS");
			TranslateControls();
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
			this.rbRemain = new System.Windows.Forms.RadioButton();
			this.btnOK = new System.Windows.Forms.Button();
			this.rbCancel = new System.Windows.Forms.RadioButton();
			this.txtMsg = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// rbRemain
			// 
			this.rbRemain.Location = new System.Drawing.Point(40, 104);
			this.rbRemain.Name = "rbRemain";
			this.rbRemain.Size = new System.Drawing.Size(320, 24);
			this.rbRemain.TabIndex = 10;
			this.rbRemain.Text = "Allow The Credit To Remain On The Account For Refund";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(176, 144);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(40, 24);
			this.btnOK.TabIndex = 9;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// rbCancel
			// 
			this.rbCancel.Checked = true;
			this.rbCancel.Location = new System.Drawing.Point(40, 64);
			this.rbCancel.Name = "rbCancel";
			this.rbCancel.Size = new System.Drawing.Size(352, 24);
			this.rbCancel.TabIndex = 8;
			this.rbCancel.TabStop = true;
			this.rbCancel.Text = "Transfer The Money To The Sundry Account && Cancel Account";
			// 
			// txtMsg
			// 
			this.txtMsg.Location = new System.Drawing.Point(80, 16);
			this.txtMsg.Name = "txtMsg";
			this.txtMsg.Size = new System.Drawing.Size(256, 32);
			this.txtMsg.TabIndex = 6;
			this.txtMsg.Text = "label1";
			// 
			// OutstandingPayment
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(400, 174);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.rbRemain,
																		  this.btnOK,
																		  this.rbCancel,
																		  this.txtMsg});
			this.Name = "OutstandingPayment";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Outstanding Payments";
			this.ResumeLayout(false);

		}
		#endregion

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			Close();
		}
	}
}
