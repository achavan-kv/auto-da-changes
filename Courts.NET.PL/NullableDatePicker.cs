using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace STL.PL
{
	/// <summary>
	/// Summary description for NullableDatePicker.
	/// </summary>
	public class NullableDatePicker : System.Windows.Forms.DateTimePicker
	{
		/// <summary> 
		/// Custom control based on the DateTimePicker which will 
		/// allow DBNull as a value.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panel = null;
		private System.Windows.Forms.Label label = null;

		public NullableDatePicker()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			/* create the blank panel */
			panel = new Panel();
			panel.Size = new Size(Size.Width - 17, Size.Height);
			panel.Location = this.Location;
			panel.BackColor = SystemColors.Control;
			this.Controls.Add(panel);
			panel.Click += new System.EventHandler(OnPanelClick);

			/* add a label to the panel */
			label = new Label();
			label.Text = "  - not set -  ";
			label.Click += new System.EventHandler(OnPanelClick);
			panel.Controls.Add(label);

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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// NullableDatePicker
			// 
			this.Name = "NullableDatePicker";
			this.Size = new System.Drawing.Size(144, 24);
			this.Click += new System.EventHandler(this.NullableDatePicker_Click);
			this.Resize += new System.EventHandler(this.NullableDatePicker_Resize);
			this.Value = DateTime.Now;

		}
		#endregion

		private void NullableDatePicker_Resize(object sender, System.EventArgs e)
		{
			panel.Size = new Size(Size.Width-17, Size.Height);
		}

		private void Initialise()
		{
			if(panel!=null)
			{
				panel.Visible = false;
				base.Value = DateTime.Now;
			}
		}

		private void NullableDatePicker_Click(object sender, System.EventArgs e)
		{
			Initialise();
		}

		private void OnPanelClick(object sender, System.EventArgs e)
		{
			Initialise();
		}

		private void Nullify()
		{
			panel.Visible = true;
		}

		[Description("Unlike the DateTimePicker the value property is an object type not a DateTime. However, it will only accept a DateTime or a DBNull.")]
		[Browsable(true)]
		public new object Value
		{
			get
			{
				if(panel.Visible)
					return DBNull.Value;
				else
					return base.Value;
			}
			set
			{
				if(value.GetType().Name == "DateTime")
				{
					Initialise();
					base.Value = (DateTime)value;
				}
				else
				{
					if(value.GetType().Name == "DBNull")
						Nullify();
				}
			}
		}

	}
}
