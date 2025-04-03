using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace STL.Common
{
	/// <summary>
	/// Summary description for AccountTextBox.
	/// </summary>
	public class ReadOnlyTextBox : System.Windows.Forms.TextBox
	{
		private System.Windows.Forms.ContextMenu contextMenu1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ReadOnlyTextBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			this.ContextMenu = contextMenu1;

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
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			// 
			// ReadOnlyTextBox
			// 
			//this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);

		}
		#endregion

		private void OnKeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = true;
		}

	}
}
