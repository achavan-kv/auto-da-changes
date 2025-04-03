using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;

namespace STL.PL
{
	/// <summary>
	/// Summary description for TransferTransaction.
	/// </summary>
	public class Refund : CommonForm
	{
		//private Crownwood.Magic.Menus.MenuControl menuMain;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Refund(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
		}

		public Refund(Form root, Form parent)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
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
			this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
			this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
			// 
			// menuFile
			// 
			this.menuFile.Description = "MenuItem";
			this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
																							this.menuExit});
			this.menuFile.Text = "&File";
			// 
			// menuExit
			// 
			this.menuExit.Description = "MenuItem";
			this.menuExit.Text = "E&xit";
			this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
			// 
			// Refund
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(792, 477);
			this.Name = "Refund";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Refunds";

		}
		#endregion

		private void menuExit_Click(object sender, System.EventArgs e)
		{
			CloseTab();
		}
	}
}
