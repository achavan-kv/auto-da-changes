using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using STL.Common.Constants.ColumnNames;

namespace STL.PL
{
	/// <summary>
	/// Summary description for CashTillOpen.
	/// </summary>
	public class CashTillOpen : CommonForm
	{
		private System.Windows.Forms.ComboBox drpReason;
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button btnOK;

		private string _reason = "";
		public string Reason 
		{
			get{return _reason;}
		}

		public CashTillOpen(TranslationDummy d)
		{
			InitializeComponent();
		}

		public CashTillOpen(DataTable reasons)
		{
			InitializeComponent();

			drpReason.DataSource = reasons;
			drpReason.DisplayMember = CN.CodeDescription;

			drpReason_SelectedIndexChanged(null, null);
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
			this.drpReason = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// drpReason
			// 
			this.drpReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpReason.Location = new System.Drawing.Point(16, 72);
			this.drpReason.Name = "drpReason";
			this.drpReason.Size = new System.Drawing.Size(160, 21);
			this.drpReason.TabIndex = 0;
			this.drpReason.SelectedIndexChanged += new System.EventHandler(this.drpReason_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(176, 40);
			this.label1.TabIndex = 1;
			this.label1.Text = "Please select the reason for opening the cash till from the list below";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(80, 112);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(40, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// CashTillOpen
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(200, 141);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btnOK,
																		  this.label1,
																		  this.drpReason});
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CashTillOpen";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Cash Till Open Reason";
			this.ResumeLayout(false);

		}
		#endregion

		private void drpReason_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				int i = drpReason.SelectedIndex;

				if(i>=0)
					_reason = (string)((DataTable)drpReason.DataSource).DefaultView[i][CN.Code];

			}
			catch(Exception ex)
			{
				Catch(ex, "drpReason_SelectedIndexChanged");
			}
			finally
			{
				StopWait();
			}
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			Close();
		}
	}
}
