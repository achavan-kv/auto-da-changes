using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace STL.PL
{
	/// <summary>
	/// Entry field for a telephone number that only allows numerics to be entered.
	/// The control will prevent non-numerics from being typed or pasted into the field.
	/// </summary>
	public class PhoneNumberBox : System.Windows.Forms.TextBox
	{
		//private bool suspend = false;
		private CommonForm form = null;
		private System.Windows.Forms.ContextMenu context = null;
		private System.Windows.Forms.MenuItem menuCopy = null;
		private System.Windows.Forms.MenuItem menuPaste = null;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PhoneNumberBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			form = new CommonForm();

		}

		private void OnMenuCopy(object sender, System.EventArgs e)
		{
			Clipboard.SetDataObject(this.Text.Replace("-",""), true);
		}

		private void OnMenuPaste(object sender, System.EventArgs e)
		{
			//suspend = true;

			string text = "";
			IDataObject data = Clipboard.GetDataObject();
			if (data.GetDataPresent(DataFormats.Text))
			{
				text = data.GetData(DataFormats.Text).ToString();
				if(form.IsNumeric(text))
					this.Text = text;
			}

			//suspend = false;
		}

		private void OnKeyPress(object sender, KeyPressEventArgs e)
		{
			//suspend = true;			

			string text = "";
			try
			{
				if(!this.ReadOnly)
				{
					if(this.SelectionLength>0)
					{
						this.SelectionLength = 0;
						this.SelectionStart = 0;
					}

					int index = SelectionStart;

					if(e.KeyChar!=22 && e.KeyChar!=3)		//CTRL-V / CTRL-C
					{
						/* Make sure the key pressed represents a number */
						if(Char.IsNumber(e.KeyChar))
						{
							if(Text.Length <= MaxLength - 1)
							{
								this.Text += e.KeyChar.ToString();
								this.SelectionLength = 0;
								this.SelectionStart = ++index;
							}
						}
					}

					if(e.KeyChar==22)
					{
						//make sure it's numeric format
						IDataObject data = Clipboard.GetDataObject();
						if (data.GetDataPresent(DataFormats.Text))
						{
							text = data.GetData(DataFormats.Text).ToString();
							if(form.IsNumeric(text))
							{
								if(text.Length>MaxLength)
									text = text.Substring(0, MaxLength);

								this.Text = text;
								this.SelectionLength = 0;
								this.SelectionStart = index + text.Length;
							}
						}
					}

					if (e.KeyChar == 8)	/*delete*/
					{
						Text = Text.Substring(0, Text.Length-1);
						SelectionStart = Text.Length;
					}
				}
				
				if(e.KeyChar==3)
				{
					Clipboard.SetDataObject(this.Text.Replace("-",""), true);
				}
			}
			catch(Exception)		//just to avoid unhandled exceptions
			{
			}
			finally
			{
				//suspend = false;
				e.Handled = true;
			}
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

			this.context = new ContextMenu();
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);

			this.menuCopy = new MenuItem();
			this.menuPaste = new MenuItem();
			this.context.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {this.menuCopy,
																					this.menuPaste});
			this.menuCopy.Index = 0;
			this.menuCopy.Text = "Copy";
			this.menuCopy.Click += new System.EventHandler(this.OnMenuCopy);

			this.menuPaste.Index = 1;
			this.menuPaste.Text = "Paste";
			this.menuPaste.Click += new System.EventHandler(this.OnMenuPaste);

			this.ContextMenu = context;
		}
		#endregion
	}
}
