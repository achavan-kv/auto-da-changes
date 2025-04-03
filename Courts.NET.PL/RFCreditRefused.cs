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
	/// Popup prompt to inform the user that credit sanctioning has failed
	/// to award the customer any credit. The popup gives the user the options
	/// to cancel the account or convert the account to HP or to manually 
	/// refer the account.
	/// </summary>
	public class RFCreditRefused : CommonForm
	{
		private System.Windows.Forms.Label txtMsg;
		public System.Windows.Forms.RadioButton rbCancel;
		public System.Windows.Forms.RadioButton rbConvert;
		private System.Windows.Forms.Button btnOK;
		public System.Windows.Forms.RadioButton rbManualRefer;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public RFCreditRefused(TranslationDummy d)
		{
			InitializeComponent();
		}
		public RFCreditRefused(string accountNo, Form root)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			FormRoot = root;
			rbManualRefer.Visible = (bool)Country[CountryParameterNames.ManualRefer];
			txtMsg.Text = GetResource("M_NORFCREDIT", new object[] {accountNo});
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
         this.txtMsg = new System.Windows.Forms.Label();
         this.rbCancel = new System.Windows.Forms.RadioButton();
         this.rbConvert = new System.Windows.Forms.RadioButton();
         this.btnOK = new System.Windows.Forms.Button();
         this.rbManualRefer = new System.Windows.Forms.RadioButton();
         this.SuspendLayout();
         // 
         // txtMsg
         // 
         this.txtMsg.Location = new System.Drawing.Point(16, 16);
         this.txtMsg.Name = "txtMsg";
         this.txtMsg.Size = new System.Drawing.Size(256, 40);
         this.txtMsg.TabIndex = 0;
         this.txtMsg.Text = "label1";
         // 
         // rbCancel
         // 
         this.rbCancel.Location = new System.Drawing.Point(64, 64);
         this.rbCancel.Name = "rbCancel";
         this.rbCancel.Size = new System.Drawing.Size(128, 24);
         this.rbCancel.TabIndex = 2;
         this.rbCancel.Text = "cancel the account";
         // 
         // rbConvert
         // 
         this.rbConvert.Checked = true;
         this.rbConvert.Location = new System.Drawing.Point(64, 104);
         this.rbConvert.Name = "rbConvert";
         this.rbConvert.Size = new System.Drawing.Size(152, 24);
         this.rbConvert.TabIndex = 3;
         this.rbConvert.TabStop = true;
         this.rbConvert.Text = "convert the account to HP";
         // 
         // btnOK
         // 
         this.btnOK.Location = new System.Drawing.Point(112, 144);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(40, 24);
         this.btnOK.TabIndex = 4;
         this.btnOK.Text = "OK";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // rbManualRefer
         // 
         this.rbManualRefer.Location = new System.Drawing.Point(64, 84);
         this.rbManualRefer.Name = "rbManualRefer";
         this.rbManualRefer.Size = new System.Drawing.Size(159, 24);
         this.rbManualRefer.TabIndex = 5;
         this.rbManualRefer.Text = "manually refer the account";
         // 
         // RFCreditRefused
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(282, 178);
         this.Controls.Add(this.rbManualRefer);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.rbConvert);
         this.Controls.Add(this.rbCancel);
         this.Controls.Add(this.txtMsg);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
         this.Name = "RFCreditRefused";
         this.Text = "Zero credit limit";
         this.ResumeLayout(false);

		}
		#endregion

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			Close();
		}
	}
}
