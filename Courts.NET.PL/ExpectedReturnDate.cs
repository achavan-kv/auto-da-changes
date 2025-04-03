using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using STL.Common.Constants.Tags;

namespace STL.PL
{
	/// <summary>
	/// Summary description for ExpectedReturnDate.
	/// </summary>
	public class ExpectedReturnDate : CommonForm
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DateTimePicker dtDate;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.ErrorProvider errors;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private XmlNode Item = null;

		public ExpectedReturnDate(XmlNode item)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			if(item.Attributes[Tags.ExpectedReturnDate].Value != "")
				dtDate.Value = Convert.ToDateTime(item.Attributes[Tags.ExpectedReturnDate].Value);
			else
				dtDate.Value = DateTime.Today;
			Item = item;
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
			this.dtDate = new System.Windows.Forms.DateTimePicker();
			this.btnOK = new System.Windows.Forms.Button();
			this.errors = new System.Windows.Forms.ErrorProvider();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(136, 40);
			this.label1.TabIndex = 0;
			this.label1.Text = "Please enter an expected return date";
			// 
			// dtDate
			// 
			this.dtDate.CustomFormat = "ddd dd MMM yyyy";
			this.dtDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtDate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.dtDate.Location = new System.Drawing.Point(16, 48);
			this.dtDate.Name = "dtDate";
			this.dtDate.Size = new System.Drawing.Size(128, 20);
			this.dtDate.TabIndex = 9;
			this.dtDate.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(64, 80);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(40, 23);
			this.btnOK.TabIndex = 10;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// errors
			// 
			this.errors.DataMember = null;
			// 
			// ExpectedReturnDate
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(168, 112);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btnOK,
																		  this.dtDate,
																		  this.label1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ExpectedReturnDate";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.ResumeLayout(false);

		}
		#endregion

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			if(dtDate.Value < DateTime.Today)
			{
				errors.SetError(dtDate, GetResource("M_DATEMUSTBEFUTURE"));
			}
			else
			{
				errors.SetError(dtDate, "");
				Item.Attributes[Tags.ExpectedReturnDate].Value = dtDate.Value.ToString();
				Close();
			}
		}
	}
}
