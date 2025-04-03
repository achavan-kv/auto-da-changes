using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace STL.PL
{
	/// <summary>
	/// A user control to highlight an input box to indicate that it is mandatory
	/// </summary>
	public class HighliteBox : System.Windows.Forms.Control
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		protected override void OnPaint(PaintEventArgs e) 
		{
			base.OnPaint(e);
			this.BackColor = Color.FromArgb(_alpha, _color);
		}

		public new Size Size
		{
			get{return base.Size;}
			set
			{
				Size s = new Size(((Size)value).Width+(2*_border), ((Size)value).Height +(2*_border));
				base.Size = s;			
			}
		}

		public new Point Location
		{
			get{return base.Location;}
			set
			{
				Point p = new Point(((Point)value).X-_border, ((Point)value).Y-_border);
				base.Location = p;
			}
		}

		private int _border = 10;
		public int Border
		{
			get{return _border;}
			set{_border = value;}
		}

		private Color _color = Color.Turquoise;
		public Color Color
		{
			get{return _color;}
			set{_color = value;}
		}

		private int _alpha = 255;
		public int Alpha
		{
			get{return _alpha;}
			set
			{
				if(value > 255)
					_alpha = 255;
				else
					if(value < 0)
					_alpha = 0;
				else
					_alpha = value;
			}
		}

		public HighliteBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);			
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
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}
