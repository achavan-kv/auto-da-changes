using System;
using System.Drawing;

namespace STL.PL
{
	/// <summary>
	/// Derivation of the System.Windows.Forms.TreeView class that displays
	/// data as a hierarchical structure. This derivation changes the 
	/// background colour to SystemColors.Highlight.
	/// </summary>
	public class TreeViewExtended : System.Windows.Forms.TreeView
	{
		public TreeViewExtended()
		{

		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			//base.OnPaint(e);
			this.BackColor = SystemColors.Highlight;
		}
	}
}
