using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace STL.PL
{
	/// <summary>
	/// Popup prompt to request the agreement number for a Cash and Go account.
	/// </summary>
	public class GetAgreementNo : CommonForm
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtAgreementNo;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.Button btnGo;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GetAgreementNo(TranslationDummy d)
		{
			InitializeComponent();
		}

		public GetAgreementNo(Form root, Form parent)
		{
			InitializeComponent();
			FormRoot = root;
			FormParent = parent;
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
			this.txtAgreementNo = new System.Windows.Forms.TextBox();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider();
			this.btnGo = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(232, 48);
			this.label1.TabIndex = 0;
			this.label1.Text = "You have selected the Cash and Go account. Please enter the agreement number you " +
				"want to view transactions and line items for:";
			// 
			// txtAgreementNo
			// 
			this.txtAgreementNo.Location = new System.Drawing.Point(16, 72);
			this.txtAgreementNo.MaxLength = 9;
			this.txtAgreementNo.Name = "txtAgreementNo";
			this.txtAgreementNo.TabIndex = 1;
			this.txtAgreementNo.Text = "";
			// 
			// btnGo
			// 
			this.btnGo.Location = new System.Drawing.Point(128, 72);
			this.btnGo.Name = "btnGo";
			this.btnGo.Size = new System.Drawing.Size(40, 20);
			this.btnGo.TabIndex = 2;
			this.btnGo.Text = "Go";
			this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(176, 72);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(48, 20);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// GetAgreementNo
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(256, 109);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btnCancel,
																		  this.btnGo,
																		  this.txtAgreementNo,
																		  this.label1});
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GetAgreementNo";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Enter Agreement Number";
			this.ResumeLayout(false);

		}
		#endregion

		private void btnGo_Click(object sender, System.EventArgs e)
		{
			try
			{
				((AccountDetails)FormParent).AgreementNo = Convert.ToInt32(txtAgreementNo.Text);
				errorProvider1.SetError(txtAgreementNo, "");
				Close();
			}
			catch(FormatException)
			{
				errorProvider1.SetError(txtAgreementNo, GetResource("M_NONNUMERIC"));
			}			
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			((AccountDetails)FormParent).Cancel = true;
			Close();
		}
	}
}
